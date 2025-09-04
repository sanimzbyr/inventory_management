(() => {
  const form = document.getElementById('general-form');
  if (!form) return;
  let dirty = false;
  form.querySelectorAll('input,textarea,select').forEach(el => {
    el.addEventListener('input', () => dirty = true);
  });
  async function saveNow() {
    if (!dirty) return;
    const fd = new FormData(form);
    const data = Object.fromEntries(fd.entries());
    data.Category = parseInt(data.Category);
    data.IsPublic = fd.get('IsPublic') === 'on';
    try {
      const res = await fetch(`/Inventories/UpdateGeneral/${window.inventoryId}`, {
        method: 'POST', headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
      });
      if (res.ok) {
        document.getElementById('autosave-status').innerText = 'Saved';
        dirty = false;
      }
    } catch (e) {}
  }
  setInterval(saveNow, 8000);
  form.addEventListener('submit', (e)=>{ e.preventDefault(); saveNow(); });
})();
