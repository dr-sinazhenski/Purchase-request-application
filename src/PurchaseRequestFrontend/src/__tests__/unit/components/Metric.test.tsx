import { render, screen } from '@testing-library/react'

import { Metric } from '../../../components/Metric/Metric'

describe('Metric', () => {
  it('renders label and value', () => {
    render(<Metric label="Total Amount" value="$45.00" />)

    expect(screen.getByText('Total Amount')).toBeInTheDocument()
    expect(screen.getByText('$45.00')).toBeInTheDocument()
  })
})
