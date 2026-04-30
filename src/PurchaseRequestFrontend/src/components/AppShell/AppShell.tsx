import {
  ClipboardCheck,
  FilePenLine,
  Grid2X2,
  PackagePlus,
} from 'lucide-react'
import type { ReactNode } from 'react'

import type { RequestRecord, Screen } from '../../types'
import './AppShell.css'

type AppShellProps = {
  children: ReactNode
  onCreate: () => void
  onOpen: (request: RequestRecord, target?: Screen) => void
  onRequests: () => void
  requestRecords: RequestRecord[]
  reviewCount: number
  screen: Screen
}

export function AppShell({
  children,
  onCreate,
  onOpen,
  onRequests,
  requestRecords,
  reviewCount,
  screen,
}: AppShellProps) {
  return (
    <div className="app-page">
      <div className="app-shell">
        <aside className="sidebar">
          <div className="sidebar-header">
            <div className="brand-mark">
              <PackagePlus size={16} strokeWidth={2.4} />
            </div>
            <span>ProcureFlow</span>
          </div>

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
              onClick={() => {
                const nextPending =
                  requestRecords.find((request) =>
                    ['New', 'Resubmitted'].includes(request.status),
                  ) ?? requestRecords[0]

                onOpen(nextPending, 'approval')
              }}
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

          <div className="sidebar-footer">
            <div className="avatar">SC</div>
            <div>
              <strong>Sarah Chen</strong>
              <span>Approver · Acme Corp</span>
            </div>
          </div>
        </aside>

        <main className="main">{children}</main>
      </div>
    </div>
  )
}
