export type Status = 'New' | 'Resubmitted' | 'Approved' | 'Rejected'

export type Screen =
  | 'requests'
  | 'create'
  | 'detail'
  | 'edit'
  | 'approval'
  | 'profile'

export type LineItem = {
  name: string
  category: string
  quantity: number
  unitPrice: number
  productId?: string
}

export type Product = {
  id: string
  name: string
  description: string
  requestTypeIds: string[]
}

export type RequestRecord = {
  id: string
  name: string
  type: string
  status: Status
  total: number
  creator: string
  initials: string
  updated: string
  description: string
  approver: string
  submitted: string
  reason?: string
  finalRejected?: boolean
  items: LineItem[]
}

export type DecisionState = 'idle' | 'approved' | 'rejected'
