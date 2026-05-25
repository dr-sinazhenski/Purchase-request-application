import type { ReactNode } from 'react'
import './Field.css'

type FieldProps = {
  children: ReactNode
  label: string
}

export function Field({ children, label }: FieldProps) {
  return (
    <div className="field">
      <span className="field-label">{label}</span>
      {children}
    </div>
  )
}
