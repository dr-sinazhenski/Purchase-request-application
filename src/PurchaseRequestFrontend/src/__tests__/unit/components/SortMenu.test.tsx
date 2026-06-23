import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'

import { SortMenu } from '../../../components/SortMenu/SortMenu'

describe('SortMenu', () => {
  it('shows the selected sort label', () => {
    render(<SortMenu onChange={jest.fn()} value="priceHigh" />)

    expect(screen.getAllByText('Price: high to low')).toHaveLength(2)
  })

  it('falls back to newest first for an unknown value', () => {
    render(
      <SortMenu
        onChange={jest.fn()}
        value={'unknown' as Parameters<typeof SortMenu>[0]['value']}
      />,
    )

    expect(screen.getAllByText('Newest first')).toHaveLength(2)
  })

  it('calls onChange when a sort option is selected', async () => {
    const user = userEvent.setup()
    const handleChange = jest.fn()
    render(<SortMenu onChange={handleChange} value="newest" />)

    await user.click(screen.getByRole('button', { name: 'Oldest first' }))

    expect(handleChange).toHaveBeenCalledWith('oldest')
  })
})
