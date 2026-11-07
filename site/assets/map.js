(() => {
  const apiBaseUrl = window.location.hostname === 'localhost'
    ? 'http://localhost:5146/api/santa'
    : 'https://api.sleighwatch.com/api/santa';
  const requestedTime = new URLSearchParams(window.location.search).get('time');
  const endpoint = requestedTime
    ? `${apiBaseUrl}/location-test?testDateTime=${encodeURIComponent(requestedTime)}`
    : apiBaseUrl;

  const map = L.map('santa-map', { zoomControl: false, worldCopyJump: true }).setView([20, 0], 2);
  L.control.zoom({ position: 'bottomright' }).addTo(map);
  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    maxZoom: 19,
    attribution: '© OpenStreetMap contributors'
  }).addTo(map);

  const sleighIcon = L.divIcon({
    className: 'sleigh-marker',
    html: '<span role="img" aria-label="Santa’s sleigh">🛷</span>',
    iconSize: [48, 48],
    iconAnchor: [24, 24]
  });

  const marker = L.marker([20, 0], { icon: sleighIcon, keyboard: false }).addTo(map);
  const title = document.querySelector('#map-title');
  const description = document.querySelector('#santa-description');
  const location = document.querySelector('#santa-location');
  const status = document.querySelector('#santa-status');
  const testTimeNote = document.querySelector('#test-time-note');

  if (requestedTime) {
    testTimeNote.hidden = false;
    testTimeNote.textContent = `Showing the test route for ${requestedTime} (UTC).`;
  }

  fetch(endpoint)
    .then(response => {
      if (!response.ok) {
        throw new Error(`The location service returned ${response.status}.`);
      }

      return response.json();
    })
    .then(santa => {
      const latitude = Math.max(-85, Math.min(85, santa.latitude));
      const longitude = santa.longitude;
      const place = santa.toCity
        ? `Travelling from ${santa.fromCity} to ${santa.toCity}`
        : santa.fromCity
          ? `${santa.fromCity}, ${santa.country}`
          : 'North Pole';

      title.textContent = santa.status === 'InFlight'
        ? 'Santa is on the move'
        : santa.status === 'Delivered'
          ? 'Santa is heading home'
          : 'Santa is at the North Pole';
      description.textContent = santa.description;
      location.textContent = place;
      status.textContent = santa.status === 'InFlight' ? 'Delivering presents' : santa.status;
      marker.setLatLng([latitude, longitude]).bindTooltip(santa.description, { direction: 'top', offset: [0, -22] }).openTooltip();
      map.setView([latitude, longitude], santa.status === 'InFlight' ? 4 : 2);
    })
    .catch(error => {
      map.removeLayer(marker);
      title.textContent = 'We could not find Santa';
      description.textContent = 'The live location service is temporarily unavailable. Please try again in a moment.';
      location.textContent = 'Unavailable';
      status.textContent = 'Unavailable';
      console.error('Unable to load Santa location.', error);
    });
})();
