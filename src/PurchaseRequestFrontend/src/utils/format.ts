export function formatMoney(value: number, currency = 'EUR') {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency,
  }).format(value)
}

export function createRequestId(count: number) {
  return `REQ-${String(37 + count).padStart(4, '0')}`
}

export function formatSubmittedDate(date: Date) {
  return new Intl.DateTimeFormat('en-US', {
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
    month: 'short',
    year: 'numeric',
  }).format(date)
}

export function getSortableDate(value: string) {
  if (value === 'Just now') {
    return Date.now()
  }

  const parsedDate = Date.parse(value)
  if (Number.isFinite(parsedDate)) {
    return parsedDate
  }

  const dateParts = value.match(/^(\d{1,2})\/(\d{1,2})\/(\d{4})/)
  if (dateParts) {
    const [, day, month, year] = dateParts
    return new Date(Number(year), Number(month) - 1, Number(day)).getTime()
  }

  return 0
}
