import { Routes, Route } from 'react-router-dom'
import StudentProfile from '../../pages/StudentProfile/StudentProfile'
import Home from '../../pages/Home'
import CoursesOfStudent from '../../pages/StudentCourses/StudentCourses'
import SignUpForLaboratoryWorkPage from '../../pages/SignUpForLaboratoryWorkPage(student)/SignUpForLaboratoryWorkPage'
import StudentCoursePerformance from '../../pages/StudentСoursePerformance/StudentCoursePerformance'
import Login from '../../pages/LoginPage/LoginPage'
import Register from '../../pages/RegistrationPage/RegistrationPage'
import Test from '../../pages/Test'
import ProtectedRoute from '../../ProtectedRoute'
// import NotificationPage from '../../pages/NotificationPage/NotificationPage'
function AppRoutes() {
    return (
        <Routes>
            {/* Доступно всем */}
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
            <Route path="/" element={<Home />} />
            <Route path="/student-profile" element={<StudentProfile />} />
            <Route path="/courses" element={<CoursesOfStudent />} />
            <Route path="/courses/:courseId" element={<SignUpForLaboratoryWorkPage />} />
            <Route path="/courses/:courseId/notifications" element={<StudentCoursePerformance />} />
            {/* <Route path="/notifications" element={<NotificationPage />} /> */}

            {/* <Route path="/slots" element={<Test />} /> */}

            {/* Защищённые маршруты
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
            /> */}
        </Routes>
    )
}

export default AppRoutes
