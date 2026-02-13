import { useMemo, useState } from 'react'
import { useApp } from '../../context/AppContext'
import { LinkButton } from '../../utils/router'

const categories = ['All', 'Fast food', 'Pizza', 'Burgers', 'Asian', 'Desserts']

export default function HomePage() {
  const { restaurants } = useApp()
  const [search, setSearch] = useState('')
  const [openOnly, setOpenOnly] = useState(false)
  const [minRating, setMinRating] = useState(0)
  const [category, setCategory] = useState('All')

  const filtered = useMemo(() => restaurants.filter((r) => {
    const q = search.toLowerCase()
    return (!q || r.name.toLowerCase().includes(q) || r.description.toLowerCase().includes(q)) && (!openOnly || r.isOpen) && r.ratingAvg >= minRating && (category === 'All' || r.description.toLowerCase().includes(category.toLowerCase()))
  }), [restaurants, search, openOnly, minRating, category])

  return <section className="card"><h2>Restaurants</h2><div className="grid"><input placeholder="Search name/cuisine" onChange={(e) => setSearch(e.target.value)} /><select onChange={(e) => setCategory(e.target.value)}>{categories.map((x) => <option key={x}>{x}</option>)}</select><label>Min rating <input type="number" min="0" max="5" step="0.1" value={minRating} onChange={(e) => setMinRating(Number(e.target.value))} /></label><label><input type="checkbox" checked={openOnly} onChange={(e) => setOpenOnly(e.target.checked)} /> Open now</label></div><div>{filtered.map((r) => <article className="restaurant" key={r.id}><img src={r.products?.[0]?.imageUrl || 'https://placehold.co/180x120'} alt={r.name} /><h3>{r.name}</h3><p>⭐ {r.ratingAvg} • ⏱ 20-40 min • {r.isOpen ? 'Open' : 'Closed'}</p><LinkButton to={`/restaurant/${r.id}`}>View menu</LinkButton></article>)}</div></section>
}
