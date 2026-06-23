import { render, screen } from '@testing-library/react'

import { StatusBadge } from '../../../components/StatusBadge/StatusBadge'

describe('StatusBadge', () => {
  it.each([
    ['New', 'status-new'],
    ['Resubmitted', 'status-resubmitted'],
    ['Approved', 'status-approved'],
    ['For Revision', 'status-for-revision'],
    ['Rejected', 'status-rejected'],
  ] as const)('renders %s status class', (status, className) => {
    render(<StatusBadge status={status} />)

    expect(screen.getByText(status)).toHaveClass('status', className)
  })
})
