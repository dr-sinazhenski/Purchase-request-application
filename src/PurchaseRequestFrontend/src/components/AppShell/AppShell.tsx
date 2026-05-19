import {
  ClipboardCheck,
  FilePenLine,
  Grid2X2,
  PackagePlus,
} from 'lucide-react'
import type { ReactNode } from 'react'

import type { Screen } from '../../types'
import './AppShell.css'

type AppShellProps = {
  children: ReactNode
  onApprovalQueue: () => void
  onCreate: () => void
  onProfile: () => void
  onRequests: () => void
  reviewCount: number
  screen: Screen
}

export function AppShell({
  children,
  onApprovalQueue,
  onCreate,
  onProfile,
  onRequests,
  reviewCount,
  screen,
}: AppShellProps) {
  return (
    <div className="app-page">
      <div className="app-shell">
        <aside className="sidebar">
          <button
            className="sidebar-header brand-link"
            onClick={onRequests}
            type="button"
          >
            <div className="brand-mark">
              <PackagePlus size={16} strokeWidth={2.4} />
            </div>
            <span>ProcureFlow</span>
          </button>

          <nav className="nav">
            <p className="nav-kicker">Overview</p>
            <button
              className={screen === 'requests' ? 'nav-item active' : 'nav-item'}
              onClick={onRequests}
              type="button"
            >
              <Grid2X2 size={16} />
              All Requests
            </button>
            <button
              className={screen === 'approval' ? 'nav-item active' : 'nav-item'}
              onClick={onApprovalQueue}
              type="button"
            >
              <ClipboardCheck size={16} />
              Approval Queue
              <span className="nav-count">{reviewCount}</span>
            </button>
            <button
              className={screen === 'create' ? 'nav-item active' : 'nav-item'}
              onClick={onCreate}
              type="button"
            >
              <FilePenLine size={16} />
              New Request
            </button>
          </nav>

          <button
            className={
              screen === 'profile'
                ? 'sidebar-footer profile-link active'
                : 'sidebar-footer profile-link'
            }
            onClick={onProfile}
            type="button"
          >
            <div className="avatar">SC</div>
            <div>
              <strong>Sarah Chen</strong>
              <span>Approver · Acme Corp</span>
            </div>
          </button>
        </aside>

        <main className="main">{children}</main>

        <nav className="mobile-nav" aria-label="Mobile navigation">
          <button
            className={screen === 'requests' ? 'mobile-nav-item active' : 'mobile-nav-item'}
            onClick={onRequests}
            type="button"
          >
            <Grid2X2 size={20} />
            <span>Requests</span>
          </button>
          <button
            className={screen === 'approval' ? 'mobile-nav-item active' : 'mobile-nav-item'}
            onClick={onApprovalQueue}
            type="button"
          >
            <span className="mobile-nav-icon-wrap">
              <ClipboardCheck size={20} />
              {reviewCount > 0 && <span className="mobile-nav-badge">{reviewCount}</span>}
            </span>
            <span>Queue</span>
          </button>
          <button
            className={screen === 'create' ? 'mobile-nav-item active' : 'mobile-nav-item'}
            onClick={onCreate}
            type="button"
          >
            <FilePenLine size={20} />
            <span>New</span>
          </button>
          <button
            className={screen === 'profile' ? 'mobile-nav-item active' : 'mobile-nav-item'}
            onClick={onProfile}
            type="button"
          >
            <span className="mobile-avatar">SC</span>
            <span>Profile</span>
          </button>
        </nav>
      </div>
    </div>
  )
}
