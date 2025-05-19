import { useLocation } from "react-router-dom"
import Header from "./components/Header/Header"
import AppRoutes from "./components/AppRoutes/AppRoutes"

function App() {
  const location = useLocation()
  const hideHeaderPaths = ['/login', '/register']
  const hideHeader = hideHeaderPaths.includes(location.pathname)

  return (
    <>
      {!hideHeader && <Header />}
      <AppRoutes />
    </>
  )
}

export default App;