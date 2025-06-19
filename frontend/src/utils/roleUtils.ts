export const translateRole = (roles: string[] = []): string => {
    const roleTranslations: Record<string, string> = {
        student: 'Студент',
        teacher: 'Преподаватель',
        administrator: 'Администратор',
    };

    const roleHierarchy: Record<string, number> = {
        administrator: 3,
        teacher: 2,
        student: 1
    };

    if (roles.length === 0) return 'Роль не указана';

    const dominantRole = roles.reduce((prev, current) => {
        const prevWeight = roleHierarchy[prev.toLowerCase()] || 0;
        const currentWeight = roleHierarchy[current.toLowerCase()] || 0;
        return currentWeight > prevWeight ? current : prev;
    }, roles[0]);

    return roleTranslations[dominantRole.toLowerCase()] || dominantRole;
};