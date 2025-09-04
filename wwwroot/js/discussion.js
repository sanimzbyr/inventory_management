(() => {
  const postsDiv = document.getElementById('posts');
  const sendBtn = document.getElementById('send');
  if (!postsDiv || !sendBtn) return;
  const conn = new signalR.HubConnectionBuilder().withUrl('/hubs/discussion').build();
  conn.on('newPost', p => addPost(p));
  conn.start();

  function addPost(p){
    const el = document.createElement('div');
    el.className = 'border rounded p-2 mb-2';
    el.innerHTML = `<div class="small text-muted">${p.author} · ${new Date(p.createdAt).toLocaleString()}</div>
      <div>${marked.parse(p.contentMd)}</div>`;
    postsDiv.appendChild(el);
  }

  sendBtn.addEventListener('click', async () => {
    const txt = document.getElementById('msg').value;
    const res = await fetch(`/api/discussion/${window.inventoryId}`, {method:'POST', headers:{'Content-Type':'application/json'}, body: JSON.stringify({text:txt})});
    document.getElementById('msg').value='';
  });

  // Load recent posts
  fetch(`/api/discussion/${window.inventoryId}`).then(r=>r.json()).then(list => list.forEach(addPost));
})();
