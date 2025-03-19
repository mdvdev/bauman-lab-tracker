from typing import Optional
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.remote.webdriver import WebDriver
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.remote.webelement import WebElement
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.chrome.options import Options
from dataclasses import dataclass, fields
import csv
import json
import os
import logging


TARGET_URL = "https://eu.bmstu.ru/modules/contingent3/"
LOGIN_URL = f"https://proxy.bmstu.ru:8443/cas/login?service={TARGET_URL}"
OUTPUT_FILE_NAME = "eu_students.csv"
CREDENTIALS_FILE_NAME = "credentials.json"
LOG_FILE_NAME = "eu_students.log"


logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s - %(levelname)s - %(message)s",
    handlers=[
        logging.StreamHandler(),  # To stdout.
        logging.FileHandler(LOG_FILE_NAME, encoding="utf-8"),  # To file.
    ],
)


logger = logging.getLogger()


@dataclass
class Student:
    name: str
    record_book_id: str
    group_id: str
    specialty_id: str

    FIELDS_MAPPING = {
        "name": "Full Name",
        "record_book_id": "Record Book ID",
        "group_id": "Group ID",
        "specialty_id": "Specialty ID",
    }

    def to_dict(self):
        return {
            self.FIELDS_MAPPING[field.name]: getattr(self, field.name)
            for field in fields(self)
        }


HEADERS = [Student.FIELDS_MAPPING[field.name] for field in fields(Student)]


def load_credentials(filename: str):
    logger.info(f"Loading credentials from file: {filename}.")

    if not os.path.exists(filename):
        raise FileNotFoundError(
            f"File with credentials not found: {filename}."
        )

    try:
        with open(filename, "r", encoding="utf-8") as file:
            credentials = json.load(file)
            if "login" not in credentials or "password" not in credentials:
                raise ValueError(f"Invalid credentials file: {filename}.")
            logger.info("Credentials loaded successfully.")
            return credentials["login"], credentials["password"]
    except json.JSONDecodeError:
        raise ValueError(f"Invalid JSON format in file: {filename}.")


def auth(driver: WebDriver, url: str):
    logger.info(f"Authenticating on the website: {url}.")
    login, password = load_credentials(CREDENTIALS_FILE_NAME)
    driver.get(url)

    # The website requires double authentication on the first
    # attempt after browser restart.
    # This is a known behavior: the form resets after the first submission.
    for i in range(2):
        logger.info(f"Trying to authenticate {i + 1}.")
        username_field = WebDriverWait(driver, 10).until(
            EC.presence_of_element_located((By.NAME, "username"))
        )
        password_field = WebDriverWait(driver, 10).until(
            EC.presence_of_element_located((By.NAME, "password"))
        )

        username_field.send_keys(login)
        password_field.send_keys(password)

        # Save URL to wait for redirection.
        current_url = driver.current_url

        login_button = driver.find_element(By.NAME, "submit")
        login_button.click()

        try:
            # Wait until the URL changes.
            WebDriverWait(driver, 10).until(EC.url_changes(current_url))
        except Exception:
            raise ValueError("Failed to authenticate.")

    WebDriverWait(driver, 10).until(EC.url_contains(TARGET_URL))
    logger.info("Successfully authenticated.")


def deserialize_html_student(
    html_student: WebElement, url: str
) -> Optional[Student]:
    tds = html_student.find_elements(By.TAG_NAME, "td")
    if not tds:
        logger.warning(f"Cannot find <td> tags in the <tr>. URL: {url}")
        return None

    # Current HTML structure contains at least 5 <td> elements in the <tr>.
    if len(tds) < 5:
        logger.warning(
            f"Invalid <td> structure. Expected 5 elements, "
            f"but found {len(tds)}. URL: {url}"
        )
        return None

    return Student(
        name=tds[1].text.strip() or "N/A",
        record_book_id=tds[2].text.strip() or "N/A",
        group_id=tds[3].text.strip() or "N/A",
        specialty_id=tds[4].text.strip() or "N/A",
    )


def parse_students(driver: WebDriver, url: str) -> list[Student]:
    logger.info(f"Parsing students from the URL: {url}.")
    driver.get(url)

    html_students = driver.find_elements(By.XPATH, "//tbody/tr")
    if not html_students:
        logger.warning(f"Cannot find <tr> tags in the <tbody>. URL: {url}")
        return []

    students = [
        student
        for html_student in html_students
        if (student := deserialize_html_student(html_student, url))
    ]

    logger.info(f"Found {len(students)} students.")
    return students


def find_department_links(driver: WebDriver) -> list[str]:
    logger.info("Finding department links.")
    first_ul = driver.find_element(By.XPATH, "//ul[@class='eu-tree-nodeset']")
    links = first_ul.find_elements(
        By.XPATH,
        (
            "./li[normalize-space(@class)='eu-tree-closed']"
            "/ul/li[normalize-space(@class)='eu-tree-closed']"
            "/ul/li[@class='eu-tree-active']/a"
        ),
    )
    if not links:
        raise ValueError(
            f"Cannot find department <a> tag in the document. "
            f"URL: {driver.current_url}"
        )

    logger.info(f"Found {len(links)} department links.")
    return [elem for link in links if (elem := link.get_attribute("href"))]


def write_to_csv(writer: csv.DictWriter, students: list[Student]):
    logger.info("Writing students to the CSV file.")
    for student in students:
        writer.writerow(student.to_dict())
    logger.info(f"Wrote {len(students)} students to the CSV file.")


def setup_driver() -> WebDriver:
    chrome_options = Options()
    # Comment this if you want to see the browser.
    # chrome_options.add_argument('--headless')
    return webdriver.Chrome(options=chrome_options)


def process_students(
    driver: WebDriver,
    links: list[str],
):
    with open(OUTPUT_FILE_NAME, "w", encoding="utf-8") as file:
        writer = csv.DictWriter(file, fieldnames=HEADERS)
        writer.writeheader()
        for link in links:
            students = parse_students(driver, link)
            write_to_csv(writer, students)


def main():
    try:
        logger.info("Starting the script.")
        with setup_driver() as driver:
            auth(driver, LOGIN_URL)
            links = find_department_links(driver)
            process_students(driver, links)
            logger.info("Script finished successfully.")
    except Exception as e:
        print(f"An error occurred: {e}")


if __name__ == "__main__":
    main()
