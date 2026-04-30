import { SlidersHorizontal } from 'lucide-react'

import { statusFilters } from '../../data/requests'
import type { RequestRecord, Screen, Status } from '../../types'
import { formatMoney } from '../../utils/format'
import { StatusBadge } from '../StatusBadge/StatusBadge'
import { Topbar } from '../Topbar/Topbar'
import './RequestsList.css'

type RequestsListProps = {
  filter: 'All' | Status
  filteredRequests: RequestRecord[]
  onCreate: () => void
  onFilter: (filter: 'All' | Status) => void
  onOpen: (request: RequestRecord, target?: Screen) => void
  onShowMore: () => void
  onTypeFilter: (type: string) => void
  totalFiltered: number
  totalRequests: number
  typeFilter: string
  uniqueTypes: string[]
  visibleCount: number
}

export function RequestsList({
  filter,
  filteredRequests,
  onCreate,
  onFilter,
  onOpen,
  onShowMore,
  onTypeFilter,
  totalFiltered,
  totalRequests,
  typeFilter,
  uniqueTypes,
  visibleCount,
}: RequestsListProps) {
  return (
    <>
      <Topbar
        count={`${totalFiltered} shown of ${totalRequests}`}
        primaryAction="+ New Request"
        title="All Requests"
        onPrimary={onCreate}
      />

      <section className="filter-bar">
        <span>Status:</span>
        {statusFilters.map((item) => (
          <button
            className={filter === item ? 'chip active' : 'chip'}
            key={item}
            onClick={() => onFilter(item)}
            type="button"
          >
            {item}
          </button>
        ))}
        <span className="divider" />
        <span>Type:</span>
        <div
          className="type-filter"
          onWheel={(event) => {
            event.preventDefault()
            event.currentTarget.scrollLeft += event.deltaY
          }}
        >
          {uniqueTypes.map((type) => (
            <button
              className={typeFilter === type ? 'chip active' : 'chip'}
              key={type}
              onClick={() => onTypeFilter(type)}
              type="button"
            >
              {type}
            </button>
          ))}
        </div>

        <button className="sort-button" type="button">
          <SlidersHorizontal size={14} />
          Sort: Newest first
        </button>
      </section>

      <section className="content-area">
        <div className="table-card">
          <table>
            <thead>
              <tr>
                <th>Request name ↑</th>
                <th>Type</th>
                <th>Status</th>
                <th>Total price</th>
                <th>Creator</th>
                <th>Last updated</th>
                <th />
              </tr>
            </thead>
            <tbody>
              {filteredRequests.map((request) => (
                <tr key={request.id}>
                  <td>
                    <button
                      className="name-button"
                      onClick={() => onOpen(request)}
                      type="button"
                    >
                      {request.name}
                    </button>
                  </td>
                  <td>{request.type}</td>
                  <td>
                    <StatusBadge
                      finalRejected={request.finalRejected}
                      status={request.status}
                    />
                  </td>
                  <td className="money">{formatMoney(request.total)}</td>
                  <td>
                    <span className="creator">
                      <span className="creator-dot">{request.initials}</span>
                      {request.creator}
                    </span>
                  </td>
                  <td className="muted">{request.updated}</td>
                  <td>
                    <button
                      className="btn compact"
                      onClick={() =>
                        onOpen(
                          request,
                          request.status === 'New' ||
                            request.status === 'Resubmitted'
                            ? 'approval'
                            : 'detail',
                        )
                      }
                      type="button"
                    >
                      {request.status === 'New' ||
                      request.status === 'Resubmitted'
                        ? 'Review'
                        : 'View'}
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        <div className="table-footer">
          <span>
            Showing {filteredRequests.length} of {totalFiltered} filtered
            requests
          </span>
          {visibleCount < totalFiltered && (
            <button className="btn compact" onClick={onShowMore} type="button">
              Show more
            </button>
          )}
        </div>
      </section>
    </>
  )
}
