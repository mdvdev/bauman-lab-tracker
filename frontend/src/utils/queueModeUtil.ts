export function getCourseQueueMode(mode: string): string {
    switch (mode) {
        case 'Democratic':
            return "Демократический";
        case 'Oligarchic':
            return "Олигархический";
        case 'Anarchic':
            return "Анархический";
        default:
            return "Не удалось распознать режим работы";
    }
}
