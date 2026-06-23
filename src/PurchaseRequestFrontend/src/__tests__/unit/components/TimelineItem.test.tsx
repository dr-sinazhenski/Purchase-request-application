import { render, screen } from '@testing-library/react'

import { TimelineItem } from '../../../components/TimelineItem/TimelineItem'

describe('TimelineItem', () => {
  it('marks active timeline items', () => {
    const { container } = render(
      <TimelineItem active label="Submitted" text="23.06.2026" />,
    )

    expect(screen.getByText('Submitted')).toBeInTheDocument()
    expect(screen.getByText('23.06.2026')).toBeInTheDocument()
    expect(container.firstChild).toHaveClass('timeline-item', 'active')
  })

  it('renders inactive timeline items', () => {
    const { container } = render(
      <TimelineItem active={false} label="Decision" text="Waiting" />,
    )

    expect(container.firstChild).toHaveClass('timeline-item')
    expect(container.firstChild).not.toHaveClass('active')
  })
})
