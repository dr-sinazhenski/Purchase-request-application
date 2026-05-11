import { Search } from 'lucide-react'
import './Topbar.css'

type TopbarProps = {
  count?: string
  primaryAction?: string
  searchPlaceholder?: string
  searchValue?: string
  title: string
  onSearch?: (value: string) => void
  onPrimary?: () => void
}

export function Topbar({
  count,
  onPrimary,
  onSearch,
  primaryAction,
  searchPlaceholder = 'Search',
  searchValue,
  title,
}: TopbarProps) {
  return (
    <header className="topbar">
      <div>
        <h1>{title}</h1>
        {count && <span>· {count}</span>}
      </div>
      <div className="topbar-actions">
        {onSearch && (
          <label className="search">
            <Search size={14} />
            <input
              aria-label="Search requests"
              onChange={(event) => onSearch(event.target.value)}
              placeholder={searchPlaceholder}
              value={searchValue ?? ''}
            />
          </label>
        )}
        {primaryAction && (
          <button className="btn primary" onClick={onPrimary} type="button">
            {primaryAction}
          </button>
        )}
      </div>
    </header>
  )
}
