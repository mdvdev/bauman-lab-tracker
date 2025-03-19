import psycopg2
import csv
from psycopg2.extensions import cursor


DB_HOST = 'localhost'
DB_NAME = 'eu_students'
DB_USER = 'postgres'
DB_PASSWORD = '0801'
IMPORT_FILE_NAME = 'eu_students.csv'


def init_database(cursor: cursor):
    with open(IMPORT_FILE_NAME, 'r', encoding='utf-8') as file:
        reader = csv.reader(file)
        next(reader)  # Skip the header row.

        for row in reader:
            full_name, record_book_id, group_id, specialty_id = row
            insert_query = '''
            INSERT INTO students (
                full_name, record_book_id, group_id, specialty_id
            )
            VALUES (%s, %s, %s, %s)
            '''

        cursor.execute(
            insert_query,
            (full_name, record_book_id, group_id, specialty_id)
        )


def main():
    try:
        with psycopg2.connect(
            host=DB_HOST,
            database=DB_NAME,
            user=DB_USER,
            password=DB_PASSWORD,
            port=5433,
        ) as connection, connection.cursor() as cursor:
            connection.autocommit = True

            create_table_query = '''
            CREATE TABLE IF NOT EXISTS students (
                id SERIAL PRIMARY KEY,
                full_name VARCHAR(255),
                record_book_id VARCHAR(50),
                group_id VARCHAR(50),
                specialty_id VARCHAR(50)
            )
            '''
            cursor.execute(create_table_query)
            print("Table created successfully or already exists.")
            init_database(cursor)

        print("Data successfully loaded from CSV.")

    except Exception as error:
        print(f"An error occured: {error}")

    print("Connection to database is closed.")


if __name__ == '__main__':
    main()
