import {
  ClipboardCheck,
  FilePenLine,
  Grid2X2,
  PackagePlus,
  Shield,
} from 'lucide-react'
import type { ReactNode } from 'react'

import type { AccountOption } from '../../api'
import type { Screen } from '../../types'
import { getPrimaryRoleName } from '../../utils/roles'
import './AppShell.css'

type AppShellProps = {
  account?: AccountOption
  canCreateRequests: boolean
  canManageAdmin: boolean
  children: ReactNode
  canReviewRequests: boolean
  onApprovalQueue: () => void
  onAdmin: () => void
  onCreate: () => void
  onProfile: () => void
  onRequests: () => void
  reviewCount: number
  screen: Screen
}

function getInitials(name = '') {
  return (
    name
      .split(' ')
      .filter(Boolean)
      .map((part) => part[0])
      .join('')
      .slice(0, 2)
      .toUpperCase() || 'CU'
  )
}

export function AppShell({
  account,
  canManageAdmin,
  canCreateRequests,
  canReviewRequests,
  children,
  onApprovalQueue,
  onAdmin,
  onCreate,
  onProfile,
  onRequests,
  reviewCount,
  screen,
}: AppShellProps) {
  const displayName = account?.name ?? 'Current User'
  const displayRole = getPrimaryRoleName(account?.roleNames)

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
            {canReviewRequests && (
              <button
                className={screen === 'approval' ? 'nav-item active' : 'nav-item'}
                onClick={onApprovalQueue}
                type="button"
              >
                <ClipboardCheck size={16} />
                Approval Queue
                <span className="nav-count">{reviewCount}</span>
              </button>
            )}
            {canCreateRequests && (
              <button
                className={screen === 'create' ? 'nav-item active' : 'nav-item'}
                onClick={onCreate}
                type="button"
              >
                <FilePenLine size={16} />
                New Request
              </button>
            )}
            {canManageAdmin && (
              <button
                className={
                  screen === 'admin' || screen === 'adminUser'
                    ? 'nav-item active'
                    : 'nav-item'
                }
                onClick={onAdmin}
                type="button"
              >
                <Shield size={16} />
                Admin
              </button>
            )}
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
            <div className="avatar">{getInitials(displayName)}</div>
            <div>
              <strong>{displayName}</strong>
              <span>
                {displayRole} - {account?.regionName ?? 'Acme Corp'}
              </span>
            </div>
          </button>
        </aside>

        <main className="main">{children}</main>

        <nav className="mobile-nav" aria-label="Mobile navigation">
          <button
            className={
              screen === 'requests' ? 'mobile-nav-item active' : 'mobile-nav-item'
            }
            onClick={onRequests}
            type="button"
          >
            <Grid2X2 size={20} />
            <span>Requests</span>
          </button>
          {canReviewRequests && (
            <button
              className={
                screen === 'approval'
                  ? 'mobile-nav-item active'
                  : 'mobile-nav-item'
              }
              onClick={onApprovalQueue}
              type="button"
            >
              <span className="mobile-nav-icon-wrap">
                <ClipboardCheck size={20} />
                {reviewCount > 0 && (
                  <span className="mobile-nav-badge">{reviewCount}</span>
                )}
              </span>
              <span>Queue</span>
            </button>
          )}
          {canCreateRequests && (
            <button
              className={
                screen === 'create' ? 'mobile-nav-item active' : 'mobile-nav-item'
              }
              onClick={onCreate}
              type="button"
            >
              <FilePenLine size={20} />
              <span>New</span>
            </button>
          )}
          {canManageAdmin && (
            <button
              className={
                screen === 'admin' || screen === 'adminUser'
                  ? 'mobile-nav-item active'
                  : 'mobile-nav-item'
              }
              onClick={onAdmin}
              type="button"
            >
              <Shield size={20} />
              <span>Admin</span>
            </button>
          )}
          <button
            className={
              screen === 'profile' ? 'mobile-nav-item active' : 'mobile-nav-item'
            }
            onClick={onProfile}
            type="button"
          >
            <span className="mobile-avatar">{getInitials(displayName)}</span>
            <span>Profile</span>
          </button>
        </nav>
      </div>
    </div>
  )
}
