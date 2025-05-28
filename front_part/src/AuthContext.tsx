import React, {
    createContext,
    useState,
    useEffect,
    ReactNode,
    useCallback,
  } from 'react';
  
  type Credentials = { email: string; password: string };
  
  type AuthContextType = {
    credentials: Credentials | null;
    isAuthenticated: boolean;
    isLoading: boolean;
    login: (email: string, password: string) => Promise<boolean>;
    logout: () => void;
  };
  
  export const AuthContext = createContext<AuthContextType>({
    credentials: null,
    isAuthenticated: false,
    isLoading: true,
    login: async () => false,
    logout: () => {},
  });
  
  export const AuthProvider = ({ children }: { children: ReactNode }) => {
    const [credentials, setCredentials] = useState<Credentials | null>(null);
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [isLoading, setIsLoading] = useState(true);
  
    useEffect(() => {
      // При загрузке проверяем localStorage
      const savedCreds = localStorage.getItem('basicCreds');
      if (savedCreds) {
        try {
          const parsed = JSON.parse(savedCreds) as Credentials;
          setCredentials(parsed);
          setIsAuthenticated(true);
        } catch (err) {
          console.error('Ошибка при чтении credentials из localStorage:', err);
          localStorage.removeItem('basicCreds');
        }
      }
      setIsLoading(false); // Инициализация завершена
    }, []);
  
    const login = useCallback(async (email: string, password: string): Promise<boolean> => {
      try {
        const res = await fetch('/api/v1/auth/login', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ email, password }),
        });
  
        if (res.ok) {
          const newCreds = { email, password };
          setCredentials(newCreds);
          setIsAuthenticated(true);
          localStorage.setItem('basicCreds', JSON.stringify(newCreds));
          return true;
        } else {
          console.warn('Ошибка логина: неверные данные');
        }
      } catch (err) {
        console.error('Ошибка логина:', err);
      }
  
      // Если что-то пошло не так
      setCredentials(null);
      setIsAuthenticated(false);
      localStorage.removeItem('basicCreds');
      return false;
    }, []);
  
    const logout = useCallback(() => {
      setCredentials(null);
      setIsAuthenticated(false);
      localStorage.removeItem('basicCreds');
    }, []);
  
    return (
      <AuthContext.Provider
        value={{
          credentials,
          isAuthenticated,
          isLoading,
          login,
          logout,
        }}
      >
        {children}
      </AuthContext.Provider>
    );
  };
  