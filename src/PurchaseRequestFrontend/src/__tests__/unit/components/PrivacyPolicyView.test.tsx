import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'

import { PrivacyPolicyView } from '../../../components/PrivacyPolicyView/PrivacyPolicyView'

describe('PrivacyPolicyView', () => {
  it('renders typical privacy policy sections', () => {
    render(<PrivacyPolicyView onBack={jest.fn()} />)

    expect(screen.getByRole('heading', { name: 'Privacy Policy' })).toBeInTheDocument()
    expect(screen.getByText('Information we collect')).toBeInTheDocument()
    expect(screen.getByText('How we use information')).toBeInTheDocument()
    expect(screen.getByText('Data sharing')).toBeInTheDocument()
    expect(screen.getByText('Retention and security')).toBeInTheDocument()
    expect(screen.getByText('Your choices')).toBeInTheDocument()
  })

  it('calls onBack from the back button', async () => {
    const user = userEvent.setup()
    const onBack = jest.fn()
    render(<PrivacyPolicyView onBack={onBack} />)

    await user.click(screen.getByRole('button', { name: '← Back to sign up' }))

    expect(onBack).toHaveBeenCalledTimes(1)
  })
})
