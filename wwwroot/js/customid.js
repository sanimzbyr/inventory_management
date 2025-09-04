(() => {
  const el = document.getElementById('elements');
  if (!el) return;
  let elements = [];
  function render() {
    el.innerHTML = '';
    elements.forEach((x, idx) => {
      const div = document.createElement('div');
      div.className = 'card p-2';
      div.draggable = true;
      div.ondragstart = e => e.dataTransfer.setData('text/plain', idx);
      div.ondrop = e => { e.preventDefault(); const from = +e.dataTransfer.getData('text/plain'); const to = idx; const moved = elements.splice(from,1)[0]; elements.splice(to,0,moved); render();};
      div.ondragover = e => e.preventDefault();
      div.innerHTML = `<div class="d-flex gap-2 align-items-center">
        <span class="badge bg-secondary">${x.type}</span>
        ${(x.type==='Fixed')?`<input class="form-control form-control-sm" placeholder="text" value="${x.text??''}" oninput="this.dataset.v=this.value">`:''}
        ${(x.type==='DateTime'||x.type==='Sequence'||x.type==='Random20')?`<input class="form-control form-control-sm" placeholder="format (e.g., yyyy or D4 or X5)" value="${x.format??''}" oninput="this.dataset.f=this.value">`:''}
        <button class="btn btn-sm btn-outline-danger">×</button>
      </div>`;
      div.querySelector('button')!.onclick = () => { elements.splice(idx,1); render(); };
      // persist edits
      div.querySelectorAll('input').forEach(inp => inp.addEventListener('change', () => {
        const t = div.querySelector('input[placeholder=\"text\"]'); if (t) x.text = t.value;
        const f = div.querySelector('input[placeholder^=\"format\"]'); if (f) x.format = f.value;
      }));
      el.appendChild(div);
    });
    document.getElementById('example')!.innerText = elements.map(x => {
      if (x.type==='Fixed') return x.text||'';
      if (x.type==='RandomD6') return '123456';
      if (x.type==='RandomD9') return '123456789';
      if (x.type==='Random20') return 'A7E3A';
      if (x.type==='Guid') return 'b0e6...';
      if (x.type==='DateTime') return (x.format||'yyyyMMdd');
      if (x.type==='Sequence') return (x.format||'D3');
      return '';
    }).join('');
  }
  window.idAdd = (t) => { elements.push({type:t, text:null, format:null}); render(); };
  window.idSave = async () => {
    await fetch(`/api/idspec/${window.inventoryId}`, { method:'POST', headers:{'Content-Type':'application/json'}, body: JSON.stringify(elements)});
    alert('Saved ID spec');
  };
  render();
})();
