import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { Lab } from '../../types/labType';
import { User } from '../../types/userType';
import { Submission } from '../../types/submssionType';
import { authFetch } from '../../utils/authFetch';
import './CourseStudentsPage.css';
import TeachersList from '../../components/TeachersList/TeachersList';
import Modal from '../../components/Modal/Modal';

interface LabStatus {
  labId: string;
  status: string;
  score: number;
  maxScore: number;
  statusName: string;
}

interface StudentWithLabData {
  id: string;
  fullName: string;
  email: string;
  group: string;
  submissions: Submission[];
  labsStatus: LabStatus[];
  totalScore: number;
  passedLabsCount: number;
  isOligarch: boolean;
}

interface GroupData {
  group: string;
  students: User[];
}

function getStatusName(status: string | undefined | null): string {
  if (!status) return 'Не отправлено';
  switch (status.toLowerCase()) {
    case 'submitted':
      return 'Отправлено';
    case 'approved':
      return 'Зачтено';
    case 'rejected':
      return 'Отклонено';
    default:
      return 'Нет';
  }
}

function CourseStudentsPage() {
  const { courseId } = useParams<{ courseId: string }>();
  const [labs, setLabs] = useState<Lab[]>([]);
  const [students, setStudents] = useState<StudentWithLabData[]>([]);
  const [teachers, setTeachers] = useState<User[]>([]);
  const [courseName, setCourseName] = useState('');
  const [isTeacher, setIsTeacher] = useState(false);
  const [loading, setLoading] = useState(true);
  const [queueMode, setQueueMode] = useState<string>('');

  const [selectedStudent, setSelectedStudent] = useState<StudentWithLabData | null>(null);
  const [showConfirmDelete, setShowConfirmDelete] = useState(false);
  const [showNotificationModal, setShowNotificationModal] = useState(false);
  const [notificationTitle, setNotificationTitle] = useState('');
  const [notificationMessage, setNotificationMessage] = useState('');

  const [showAddStudentModal, setShowAddStudentModal] = useState(false);
  const [groups, setGroups] = useState<GroupData[]>([]);
  const [selectedGroup, setSelectedGroup] = useState<GroupData | null>(null);
  const [selectedGroupStudent, setSelectedGroupStudent] = useState<User | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      if (!courseId) return;

      const [labsRes, studentsRes, submissionsRes, courseRes, teachersRes, meRes] = await Promise.all([
        authFetch(`/api/v1/courses/${courseId}/labs`),
        authFetch(`/api/v1/courses/${courseId}/students`),
        authFetch(`/api/v1/courses/${courseId}/submissions`),
        authFetch(`/api/v1/courses/${courseId}`),
        authFetch(`/api/v1/courses/${courseId}/teachers`),
        authFetch(`/api/v1/users/me`)
      ]);

      const labsData: Lab[] = await labsRes.json();
      const studentsData = await studentsRes.json();
      const submissionsData: Submission[] = await submissionsRes.json();
      const courseData = await courseRes.json();
      setQueueMode(courseData.queueMode);
      const teachersData = await teachersRes.json();
      const me = await meRes.json();

      const studentList: StudentWithLabData[] = studentsData.map((s: any) => {
        const user = s.user;
        const fullName = `${user.lastName} ${user.firstName} ${user.patronymic}`.trim();
        const studentSubmissions = submissionsData.filter((sub) => sub.student.id === user.id);

        const labsStatus: LabStatus[] = labsData.map((lab) => {
          const submission = studentSubmissions.find((s) => String(s.lab.id) === String(lab.id));
          let status = submission?.submissionStatus;
          const score = submission?.score ?? 0;
          const maxScore = lab.score ?? 0;

          if (!status && score === maxScore && score > 0) {
            status = 'Approved';
          }

          return {
            labId: lab.id,
            status: status ?? '',
            statusName: getStatusName(status),
            score,
            maxScore
          };
        });

        const passedLabsCount = labsStatus.filter((l) => l.status.toLowerCase() === 'approved').length;
        const totalScore = labsStatus.reduce((sum, l) => sum + (l.score || 0), 0);

        return {
          id: user.id,
          fullName,
          email: user.email,
          group: user.group,
          submissions: studentSubmissions,
          labsStatus,
          passedLabsCount,
          totalScore,
          isOligarch: s.isOligarch
        };
      });

      setLabs(labsData);
      setStudents(studentList);
      setCourseName(courseData.name);
      setTeachers(teachersData.map((t: any) => t.user));
      setIsTeacher(me.roles.includes('Teacher') || me.roles.includes('Administrator'));
      setLoading(false);
    };

    fetchData();
  }, [courseId]);

  useEffect(() => {
    if (showAddStudentModal) {
      authFetch('/api/v1/users/groups')
        .then(res => res.json())
        .then(setGroups)
        .catch(err => console.error('Ошибка загрузки групп:', err));
    }
  }, [showAddStudentModal]);

  const handleAddStudent = async () => {
    if (!courseId || !selectedGroupStudent) return;
    await authFetch(`/api/v1/courses/${courseId}/students/${selectedGroupStudent.id}`, {
      method: 'POST'
    });
    setShowAddStudentModal(false);
    setSelectedGroup(null);
    setSelectedGroupStudent(null);
    window.location.reload(); // или вручную обновить список
  };

  const handleDelete = async () => {
    if (!selectedStudent || !courseId) return;
    await authFetch(`/api/v1/courses/${courseId}/students/${selectedStudent.id}`, { method: 'DELETE' });
    setStudents((prev) => prev.filter((s) => s.id !== selectedStudent.id));
    setShowConfirmDelete(false);
    setSelectedStudent(null);
  };

  const handleSendNotification = async () => {
    if (!selectedStudent || !courseId) return;

    const body = {
      userId: selectedStudent.id,
      title: notificationTitle,
      message: notificationMessage,
      type: 'System',
      relatedEntityId: courseId,
      relatedEntityType: 'Course'
    };

    await authFetch(`/api/v1/courses/${courseId}/students/${selectedStudent.id}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(body)
    });

    setShowNotificationModal(false);
    setNotificationTitle('');
    setNotificationMessage('');
    setSelectedStudent(null);
  };

  const toggleOligarch = async (studentId: string, currentStatus: boolean) => {
    if (!courseId) return;
  
    try {
      await authFetch(`/api/v1/courses/${courseId}/students/${studentId}?isOligarch=${!currentStatus}`, {
        method: 'PATCH',
      });
  
      // Обновить локальный стейт, чтобы изменить кнопку и статус студента
      setStudents((prevStudents) =>
        prevStudents.map((student) =>
          student.id === studentId ? { ...student, isOligarch: !currentStatus } : student
        )
      );
    } catch (error) {
      console.error('Ошибка при смене статуса олигарха:', error);
    }
  };

  if (loading) return <div>Загрузка...</div>;

  return (
    <div className="course-students-page">
      <div className="course-students-header">
        <h1 className="course-students-title">{courseName}</h1>
        {isTeacher && <button onClick={() => setShowAddStudentModal(true)}>Добавить студента</button>}
      </div>

      <TeachersList teachers={teachers} />

      <div className="students-block">
        {students.map((student) => (
          <div key={student.id} className="student-card">
            <div className="student-card-info">
              <p className="student-card-name">{student.fullName}</p>
              <p>Почта: {student.email}</p>
              <p>Группа: {student.group}</p>
              <p>Сдано лабораторных: {student.passedLabsCount}</p>
              <p>Общий балл: {student.totalScore}</p>
              {isTeacher && (
                <div className="student-actions">
                  <button className="delete" onClick={() => {
                    setSelectedStudent(student);
                    setShowConfirmDelete(true);
                  }}>Удалить</button>
                  <button className="notify" onClick={() => {
                    setSelectedStudent(student);
                    setShowNotificationModal(true);
                  }}>Отправить уведомление</button>
                  {isTeacher && queueMode === 'Oligarchic' && (
                  <button
                    className="oligarch-toggle"
                    onClick={() => toggleOligarch(student.id, student.isOligarch)}
                  >
                    {student.isOligarch ? 'Разжаловать' : 'Сделать олигархом'}
                  </button>
                )}
                </div>
              )}
            </div>

            <div className="student-labs-status">
              {labs.map((lab) => {
                const labStatus = student.labsStatus.find((l) => l.labId === lab.id);
                const isPassed = labStatus?.status?.toLowerCase() === 'approved';
                return (
                  <div key={lab.id} className="student-lab-card">
                    <p className="lab-card-title">{lab.name}</p>
                    <p className={`lab-status ${isPassed ? 'lab-status-passed' : `lab-status-${labStatus?.status?.toLowerCase() ?? 'not-submitted'}`}`}>
                      Статус: {labStatus?.statusName ?? 'Нет'}
                    </p>
                    <p className={isPassed ? 'lab-score-passed' : ''}>Баллы: {labStatus?.score ?? 0}</p>
                    <p>Макс. баллы: {lab.score}</p>
                  </div>
                );
              })}
            </div>
          </div>
        ))}
      </div>

      {/* Модальное окно удаления */}
      {showConfirmDelete && selectedStudent && (
        <Modal onClose={() => setShowConfirmDelete(false)}>
          <h2>Удалить студента {selectedStudent.fullName}?</h2>
          <div className="modal-buttons">
            <button onClick={handleDelete}>Удалить</button>
            <button onClick={() => setShowConfirmDelete(false)}>Отмена</button>
          </div>
        </Modal>
      )}

      {/* Модальное окно уведомления */}
      {showNotificationModal && selectedStudent && (
        <Modal onClose={() => setShowNotificationModal(false)}>
          <h2>Уведомление студенту {selectedStudent.fullName}</h2>
          <input placeholder="Заголовок" value={notificationTitle} onChange={(e) => setNotificationTitle(e.target.value)} />
          <input placeholder="Сообщение" value={notificationMessage} onChange={(e) => setNotificationMessage(e.target.value)} />
          <div className="modal-buttons">
            <button onClick={handleSendNotification}>Отправить</button>
            <button onClick={() => setShowNotificationModal(false)}>Отмена</button>
          </div>
        </Modal>
      )}

      {/* Модальное окно добавления */}
      {showAddStudentModal && (
        <Modal onClose={() => setShowAddStudentModal(false)}>
          <h2>Добавить студента</h2>
          <select value={selectedGroup?.group ?? ''} onChange={(e) => {
            const group = groups.find(g => g.group === e.target.value);
            setSelectedGroup(group || null);
            setSelectedGroupStudent(null);
          }}>
            <option value="">Выберите группу</option>
            {groups.map(g => (
              <option key={g.group} value={g.group}>{g.group}</option>
            ))}
          </select>

          {selectedGroup && (
            <select value={selectedGroupStudent?.id ?? ''} onChange={(e) => {
              const student = selectedGroup.students.find(s => s.id === e.target.value);
              setSelectedGroupStudent(student || null);
            }}>
              <option value="">Выберите студента</option>
              {selectedGroup.students.map(s => (
                <option key={s.id} value={s.id}>{s.lastName} {s.firstName}</option>
              ))}
            </select>
          )}

          <div className="modal-buttons">
            <button disabled={!selectedGroupStudent} onClick={handleAddStudent}>Добавить</button>
            <button onClick={() => setShowAddStudentModal(false)}>Отмена</button>
          </div>
        </Modal>
      )}
    </div>
  );
}

export default CourseStudentsPage;