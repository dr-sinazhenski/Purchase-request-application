import type { ReactNode } from 'react'
import './Field.css'

type FieldProps = {
  children: ReactNode
  label: string
}

export function Field({ children, label }: FieldProps) {
  return (
    <label className="field">
      <span>{label}</span>
      {children}
    </label>
  )
}
