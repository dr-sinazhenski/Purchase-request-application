import './TimelineItem.css'

type TimelineItemProps = {
  active: boolean
  label: string
  text: string
}

export function TimelineItem({ active, label, text }: TimelineItemProps) {
  return (
    <div className={active ? 'timeline-item active' : 'timeline-item'}>
      <span />
      <div>
        <strong>{label}</strong>
        <p>{text}</p>
      </div>
    </div>
  )
}
