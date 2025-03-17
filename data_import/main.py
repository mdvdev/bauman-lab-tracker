import psycopg2
import csv

DB_HOST = 'localhost'
DB_NAME = 'eu_students'
DB_USER = 'postgres'
DB_PASSWORD = '0801'

try:
    connection = psycopg2.connect(
        host=DB_HOST,
        database=DB_NAME,
        user=DB_USER,
        password=DB_PASSWORD,
        port=5433
    )
    connection.autocommit = True
    cursor = connection.cursor()

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
    print("Таблица создана или уже существует.")

    with open('eu_students.csv', 'r', encoding='utf-8') as file:
        reader = csv.reader(file)
        next(reader)

        for row in reader:
            full_name, record_book_id, group_id, specialty_id = row
            insert_query = '''
            INSERT INTO students (full_name, record_book_id, group_id, specialty_id)
            VALUES (%s, %s, %s, %s)
            '''
            cursor.execute(insert_query, (full_name, record_book_id, group_id, specialty_id))

    print("Данные успешно загружены из CSV.")

except Exception as error:
    print(f"Ошибка: {error}")
finally:
    if connection:
        cursor.close()
        connection.close()
        print("Соединение с базой данных закрыто.")