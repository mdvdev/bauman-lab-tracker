// src/utils/authFetch.ts
export async function authFetch(input: RequestInfo, init?: RequestInit): Promise<Response> {
    const response = await fetch(input, {
        ...init,
        credentials: 'include', // важно для Cookie-based Auth (Basic/Cookie)
        headers: {
            ...(init?.headers || {}),
            'Content-Type': 'application/json',
        },
    });

    if (response.status === 401) {
        // Перенаправление на /login
        window.location.href = '/login';
        return Promise.reject(new Error('Unauthorized'));
    }

    return response;
}
