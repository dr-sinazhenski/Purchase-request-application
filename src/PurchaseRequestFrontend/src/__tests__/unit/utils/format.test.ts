import {
  createRequestId,
  formatMoney,
  formatSubmittedDate,
  getSortableDate,
} from '../../../utils/format'

describe('format utilities', () => {
  it('formats money with EUR by default', () => {
    expect(formatMoney(45)).toBe('€45.00')
  })

  it('formats money with a provided currency', () => {
    expect(formatMoney(1200, 'USD')).toBe('$1,200.00')
    expect(formatMoney(38.5, 'GBP')).toBe('£38.50')
  })

  it('creates stable request ids from a count', () => {
    expect(createRequestId(0)).toBe('REQ-0037')
    expect(createRequestId(12)).toBe('REQ-0049')
    expect(createRequestId(1200)).toBe('REQ-1237')
  })

  it('formats submitted dates for display', () => {
    const date = new Date('2026-06-23T10:15:00Z')

    expect(formatSubmittedDate(date)).toContain('2026')
    expect(formatSubmittedDate(date)).toContain('Jun')
  })

  it('sorts Just now as the current timestamp', () => {
    jest.spyOn(Date, 'now').mockReturnValue(123456)

    expect(getSortableDate('Just now')).toBe(123456)
  })

  it('parses browser-readable date values', () => {
    expect(getSortableDate('2026-06-23T10:15:00Z')).toBe(
      Date.parse('2026-06-23T10:15:00Z'),
    )
  })

  it('parses dd/mm/yyyy date values', () => {
    expect(getSortableDate('23/06/2026')).toBe(
      new Date(2026, 5, 23).getTime(),
    )
  })

  it('returns 0 for unknown dates', () => {
    expect(getSortableDate('not a date')).toBe(0)
  })
})
