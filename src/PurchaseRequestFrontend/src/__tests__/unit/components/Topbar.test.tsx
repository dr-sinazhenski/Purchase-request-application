import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'

import { Topbar } from '../../../components/Topbar/Topbar'

describe('Topbar', () => {
  it('renders title, count, search, and primary action', async () => {
    const user = userEvent.setup()
    const onSearch = jest.fn()
    const onPrimary = jest.fn()

    render(
      <Topbar
        count="3"
        onPrimary={onPrimary}
        onSearch={onSearch}
        primaryAction="+ New Request"
        searchPlaceholder="Search requests"
        searchValue=""
        title="All Requests"
      />,
    )

    expect(screen.getByRole('heading', { name: 'All Requests' })).toBeInTheDocument()
    expect(screen.getByText('· 3')).toBeInTheDocument()

    await user.type(screen.getByLabelText('Search requests'), 'keyboard')
    expect(onSearch).toHaveBeenLastCalledWith('d')

    await user.click(screen.getByRole('button', { name: '+ New Request' }))
    expect(onPrimary).toHaveBeenCalledTimes(1)
  })

  it('omits optional controls when handlers are absent', () => {
    render(<Topbar title="Profile" />)

    expect(screen.queryByLabelText('Search requests')).not.toBeInTheDocument()
    expect(screen.queryByRole('button', { name: '+ New Request' })).not.toBeInTheDocument()
  })
})
