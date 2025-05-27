import { useLocation } from "react-router-dom";
import Header from "./components/Header/Header";
import AppRoutes from "./components/AppRoutes/AppRoutes";
import { AuthProvider } from "./AuthContext";

function App() {
  const location = useLocation();
  const hideHeaderPaths = ['/login', '/register'];
  const hideHeader = hideHeaderPaths.includes(location.pathname);

  return (
    <AuthProvider>
      {!hideHeader && <Header />}
      <AppRoutes />
    </AuthProvider>
  );
}

export default App;
