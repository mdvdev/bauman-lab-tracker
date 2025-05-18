export const translateRole = (role: string): string => {
    const roleTranslations: Record<string, string> = {
        student: 'Студент',
        teacher: 'Преподаватель',
        admin: 'Администратор',
    };

    return roleTranslations[role.toLowerCase()] || role;
};
