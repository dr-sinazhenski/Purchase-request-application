import type { Status } from '../../types'
import './StatusBadge.css'

type StatusBadgeProps = {
  finalRejected?: boolean
  status: Status
}

export function StatusBadge({ status }: StatusBadgeProps) {
  const statusClass = `status-${status.toLowerCase().replace(/\s+/g, '-')}`

  return <span className={`status ${statusClass}`}>{status}</span>
}
