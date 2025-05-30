import { Routes, Route } from 'react-router-dom';
import StudentProfile from '../../pages/StudentProfile/StudentProfile';
import CoursesOfStudent from '../../pages/StudentCourses/StudentCourses';
import SignUpForLaboratoryWorkPage from '../../pages/SignUpForLaboratoryWorkPage(student)/SignUpForLaboratoryWorkPage';
import StudentCoursePerformance from '../../pages/StudentCoursePerformance/StudentCoursePerformance';
import Login from '../../pages/LoginPage/LoginPage';
import Register from '../../pages/RegistrationPage/RegistrationPage';
import NotificationPage from '../../pages/NotificationPage/NotificationPage';
import Test from '../../pages/Test';
import ProtectedRoute from '../../ProtectedRoute';
import DetailedSlotPage from '../../pages/DetailedSlotPage/DetailedSlotPage';
import CourseStudentsPage from '../../pages/CourseStudentsPage/CourseStudentsPage';

function AppRoutes() {
    return (
        <Routes>
            {/* ✅ Открытые страницы */}
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />

            {/* ✅ Защищённые страницы */}
            <Route
                path="/student-profile"
                element={
                    <ProtectedRoute>
                        <StudentProfile />
                    </ProtectedRoute>
                }
            />
            <Route
                path="/courses"
                element={
                    <ProtectedRoute>
                        <CoursesOfStudent />
                    </ProtectedRoute>
                }
            />
            <Route
                path="/courses/:courseId"
                element={
                    <ProtectedRoute>
                        <SignUpForLaboratoryWorkPage />
                    </ProtectedRoute>
                }
            />
            <Route
                path="/courses/:courseId/notifications"
                element={
                    <ProtectedRoute>
                        <StudentCoursePerformance />
                    </ProtectedRoute>
                }
            />
            <Route
                path="/notifications"
                element={
                    <ProtectedRoute>
                        <NotificationPage />
                    </ProtectedRoute>
                }
            />
            <Route
                path="/slots"
                element={
                    <ProtectedRoute>
                        <Test />
                    </ProtectedRoute>
                }
            />

            <Route
                path="/courses/:courseId/students"
                element={
                    <ProtectedRoute>
                        <CourseStudentsPage />
                    </ProtectedRoute>
                }
            />

            <Route
                path="/courses/:courseId/slots/:slotId"
                element={
                    <ProtectedRoute>
                        <DetailedSlotPage />
                    </ProtectedRoute>
                }
            />
        </Routes>
    );
}

export default AppRoutes;
