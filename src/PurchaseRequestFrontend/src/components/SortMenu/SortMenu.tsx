import { ChevronDown, SlidersHorizontal } from 'lucide-react'

import type { RequestSort } from '../../types'
import './SortMenu.css'

const sortOptions: { label: string; value: RequestSort }[] = [
  { label: 'Newest first', value: 'newest' },
  { label: 'Oldest first', value: 'oldest' },
  { label: 'Price: high to low', value: 'priceHigh' },
  { label: 'Price: low to high', value: 'priceLow' },
]

type SortMenuProps = {
  onChange: (sort: RequestSort) => void
  value: RequestSort
}

export function SortMenu({ onChange, value }: SortMenuProps) {
  const selectedOption =
    sortOptions.find((option) => option.value === value) ?? sortOptions[0]

  return (
    <details className="sort-menu">
      <summary aria-label="Sort requests">
        <SlidersHorizontal size={15} />
        <span>{selectedOption.label}</span>
        <ChevronDown className="sort-menu-chevron" size={18} />
      </summary>
      <div className="sort-menu-options">
        {sortOptions.map((option) => (
          <button
            className={option.value === value ? 'active' : undefined}
            key={option.value}
            onClick={(event) => {
              onChange(option.value)
              event.currentTarget.closest('details')?.removeAttribute('open')
            }}
            type="button"
          >
            {option.label}
          </button>
        ))}
      </div>
    </details>
  )
}
