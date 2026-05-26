import { statusFilters } from '../../data/requests'
import type { RequestRecord, RequestSort, Screen, Status } from '../../types'
import { formatMoney } from '../../utils/format'
import { SortMenu } from '../SortMenu/SortMenu'
import { StatusBadge } from '../StatusBadge/StatusBadge'
import { Topbar } from '../Topbar/Topbar'
import './RequestsList.css'

type RequestsListProps = {
  filter: 'All' | Status
  filteredRequests: RequestRecord[]
  canCreateRequests: boolean
  canReviewRequests: boolean
  onCreate: () => void
  onFilter: (filter: 'All' | Status) => void
  onOpen: (request: RequestRecord, target?: Screen) => void
  onSearch: (value: string) => void
  onShowMore: () => void
  onSort: (sort: RequestSort) => void
  onTypeFilter: (type: string) => void
  searchQuery: string
  sort: RequestSort
  totalFiltered: number
  totalRequests: number
  typeFilter: string
  uniqueTypes: string[]
  visibleCount: number
}

export function RequestsList({
  canCreateRequests,
  canReviewRequests,
  filter,
  filteredRequests,
  onCreate,
  onFilter,
  onOpen,
  onSearch,
  onShowMore,
  onSort,
  onTypeFilter,
  searchQuery,
  sort,
  totalFiltered,
  totalRequests,
  typeFilter,
  uniqueTypes,
  visibleCount,
}: RequestsListProps) {
  return (
    <>
      <Topbar
        primaryAction={canCreateRequests ? '+ New Request' : undefined}
        searchPlaceholder="Search requests"
        searchValue={searchQuery}
        title="All Requests"
        onPrimary={onCreate}
        onSearch={onSearch}
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

        <span className="divider" />
        <SortMenu onChange={onSort} value={sort} />
      </section>

      <section className="mobile-filter-drawers" aria-label="Request filters">
        <details className="mobile-filter-drawer">
          <summary>
            <span>Status</span>
            <strong>{filter}</strong>
          </summary>
          <div className="mobile-filter-options">
            {statusFilters.map((item) => (
              <button
                className={
                  filter === item
                    ? 'mobile-filter-option active'
                    : 'mobile-filter-option'
                }
                key={item}
                onClick={() => onFilter(item)}
                type="button"
              >
                {item}
              </button>
            ))}
          </div>
        </details>

        <details className="mobile-filter-drawer">
          <summary>
            <span>Type</span>
            <strong>{typeFilter}</strong>
          </summary>
          <div className="mobile-filter-options">
            {uniqueTypes.map((type) => (
              <button
                className={
                  typeFilter === type
                    ? 'mobile-filter-option active'
                    : 'mobile-filter-option'
                }
                key={type}
                onClick={() => onTypeFilter(type)}
                type="button"
              >
                {type}
              </button>
            ))}
          </div>
        </details>

        <SortMenu onChange={onSort} value={sort} />
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
              {filteredRequests.length === 0 && (
                <tr>
                  <td className="empty-table-cell" colSpan={7}>
                    No requests match your search or filters.
                  </td>
                </tr>
              )}
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
                          canReviewRequests &&
                            (request.status === 'New' ||
                              request.status === 'Resubmitted')
                            ? 'approval'
                            : 'detail',
                        )
                      }
                      type="button"
                    >
                      {canReviewRequests &&
                      (request.status === 'New' ||
                        request.status === 'Resubmitted')
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
