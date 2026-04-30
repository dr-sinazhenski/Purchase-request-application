import type { Status } from '../../types'
import './StatusBadge.css'

type StatusBadgeProps = {
  finalRejected?: boolean
  status: Status
}

export function StatusBadge({ finalRejected = false, status }: StatusBadgeProps) {
  const label = finalRejected && status === 'Rejected' ? 'Final rejected' : status

  return <span className={`status status-${status.toLowerCase()}`}>{label}</span>
}
