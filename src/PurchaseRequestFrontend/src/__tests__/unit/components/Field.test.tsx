import { render, screen } from '@testing-library/react'

import { Field } from '../../../components/Field/Field'

describe('Field', () => {
  it('renders a label and children', () => {
    render(
      <Field label="Request Name">
        <input aria-label="Request Name input" />
      </Field>,
    )

    expect(screen.getByText('Request Name')).toBeInTheDocument()
    expect(screen.getByLabelText('Request Name input')).toBeInTheDocument()
  })
})
