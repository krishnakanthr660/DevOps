import { useEffect, useState } from "react";
import axios from "axios";

const API = import.meta.env.VITE_API_BASE_URL || "/api/contacts";

export default function App()
{
  const [contacts, setContacts] = useState([]);
  const [form, setForm] = useState({
    name: "",
    phoneNumber: "",
    country: "",
    gender: ""
  });

  useEffect(() =>
  {
    loadContacts();
  }, []);

  const loadContacts = async () =>
  {
    const res = await axios.get(API);
    setContacts(res.data);
  };

  const submit = async () =>
  {
    await axios.post(API, form);
    setForm({ name: "", phoneNumber: "", country: "", gender: "" });
    loadContacts();
  };

  const remove = async (id) =>
  {
    await axios.delete(`${API}/${id}`);
    loadContacts();
  };

  return (
    <div style={{ padding: 30 }}>
      <h2>Contact Manager</h2>

      <input placeholder="Name" value={form.name}
        onChange={e => setForm({ ...form, name: e.target.value })} />

      <input placeholder="Phone" value={form.phoneNumber}
        onChange={e => setForm({ ...form, phoneNumber: e.target.value })} />

      <input placeholder="Country" value={form.country}
        onChange={e => setForm({ ...form, country: e.target.value })} />

      <input placeholder="Gender" value={form.gender}
        onChange={e => setForm({ ...form, gender: e.target.value })} />

      <button onClick={submit}>Add</button>

      <ul>
        {contacts.map(c => (
          <li key={c.id}>
            {c.name} - {c.phoneNumber} ({c.country}, {c.gender})
            <button onClick={() => remove(c.id)}>Delete</button>
          </li>
        ))}
      </ul>
    </div>
  );
}
