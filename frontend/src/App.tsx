import { NavLink, Navigate, Route, Routes } from 'react-router-dom'
import { PessoasPage } from './pages/PessoasPage'
import { TotaisPage } from './pages/TotaisPage'
import { TransacoesPage } from './pages/TransacoesPage'

const links = [
  { to: '/pessoas', label: 'Pessoas', icon: 'P' },
  { to: '/transacoes', label: 'Transações', icon: 'T' },
  { to: '/totais', label: 'Resumo', icon: 'R' },
]

export default function App() {
  return (
    <div className="app-shell">
      <aside className="sidebar">
        <NavLink className="brand" to="/totais" aria-label="Homý — início">
          <span className="brand__mark">H</span>
          <span><strong>homý</strong><small>finanças da casa</small></span>
        </NavLink>
        <nav aria-label="Navegação principal">
          {links.map((link) => (
            <NavLink key={link.to} to={link.to} className={({ isActive }) => isActive ? 'nav-link active' : 'nav-link'}>
              <span className="nav-link__icon">{link.icon}</span>{link.label}
            </NavLink>
          ))}
        </nav>
        <p className="sidebar__note">Organize o presente.<br />Planeje com leveza.</p>
      </aside>
      <main className="main-content">
        <Routes>
          <Route path="/" element={<Navigate to="/totais" replace />} />
          <Route path="/pessoas" element={<PessoasPage />} />
          <Route path="/transacoes" element={<TransacoesPage />} />
          <Route path="/totais" element={<TotaisPage />} />
          <Route path="*" element={<Navigate to="/totais" replace />} />
        </Routes>
      </main>
    </div>
  )
}
