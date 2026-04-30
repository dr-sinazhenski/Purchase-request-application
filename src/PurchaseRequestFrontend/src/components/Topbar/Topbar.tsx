import { Search } from 'lucide-react'
import './Topbar.css'

type TopbarProps = {
  count?: string
  primaryAction?: string
  title: string
  onPrimary?: () => void
}

export function Topbar({ count, onPrimary, primaryAction, title }: TopbarProps) {
  return (
    <header className="topbar">
      <div>
        <h1>{title}</h1>
        {count && <span>· {count}</span>}
      </div>
      <div className="topbar-actions">
        <label className="search">
          <Search size={14} />
          <input defaultValue="MacBook" aria-label="Search requests" />
        </label>
        {primaryAction && (
          <button className="btn primary" onClick={onPrimary} type="button">
            {primaryAction}
          </button>
        )}
      </div>
    </header>
  )
}
