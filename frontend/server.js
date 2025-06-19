import * as jsonServer from 'json-server';
import { defaults, rewriter } from 'json-server';
import path from 'path';
import fs from 'fs';

// Создаем сервер
const server = jsonServer.create();
const router = jsonServer.router('db.json');
const middlewares = defaults();

// Загружаем маршруты из файла
const routes = JSON.parse(fs.readFileSync(path.resolve('routes.json'), 'utf-8'));

// Используем промежуточные обработчики и маршруты
server.use(middlewares);
server.use(rewriter(routes)); // Подключаем маршруты

// Добавляем кастомный маршрут для /courses/:id/slots
server.get('/courses/:id/slots', (req, res) => {
    const courseId = req.params.id;
    const db = router.db; // Получаем доступ к базе данных
    const course = db.get('courses').find({ id: courseId }).value();

    if (course) {
        const slots = db.get('slots').filter({ teacherId: course.teacherIds[0] }).value();
        return res.json(slots);
    } else {
        return res.status(404).json({ message: 'Course not found' });
    }
});

// Используем основной роутер
server.use(router);

// Запускаем сервер
server.listen(3001, () => {
    console.log('JSON Server is running on http://localhost:3001');
});
