document.querySelector('.menu')?.addEventListener('click', event => { const button = event.currentTarget; const open = button.getAttribute('aria-expanded') === 'true'; button.setAttribute('aria-expanded', String(!open)); document.body.classList.toggle('nav-open', !open); });

(() => {
  const mapElement = document.querySelector('#santa-map');
  if (!mapElement || !window.L) return;

  const apiBase = 'https://api.sleighwatch.com/api/santa';
  const params = new URLSearchParams(window.location.search);
  const requestedTime = params.get('time');
  const map = L.map(mapElement, { scrollWheelZoom: false }).setView([24, 0], 2);
  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    maxZoom: 18,
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
  }).addTo(map);

  const markerIcon = L.divIcon({
    className: '',
    html: '<span class="santa-marker" aria-hidden="true">🛷</span>',
    iconSize: [42, 42],
    iconAnchor: [21, 21]
  });
  const marker = L.marker([90, 0], { icon: markerIcon, title: 'Santa’s location' }).addTo(map);
  const statusText = document.querySelector('#santa-status');
  const locationText = document.querySelector('#santa-location');
  const descriptionText = document.querySelector('#santa-description');
  const updatedText = document.querySelector('#tracker-updated');
  const statusDot = document.querySelector('#status-dot');
  const timeInput = document.querySelector('#santa-time');
  const clearTime = document.querySelector('#clear-time');
  const timeHint = document.querySelector('#time-hint');
  let route = [];
  let pinnedTime = requestedTime;

  const setMessage = (message) => {
    updatedText.textContent = message;
    statusText.textContent = 'Location unavailable';
    locationText.textContent = 'Santa’s location could not be loaded';
    descriptionText.textContent = 'Please try again shortly.';
    statusDot.classList.remove('is-live');
  };

  const stopFor = (city) => route.find(stop => stop.city?.toLocaleLowerCase() === city?.toLocaleLowerCase());
  const positionFor = (santa) => {
    const status = (santa.status || '').toLowerCase();
    if (status === 'northpole') return [90, 0];
    if (status === 'delivered') {
      const last = route.at(-1);
      return last ? [last.latitude, last.longitude] : [90, 0];
    }

    const from = stopFor(santa.fromCity);
    const to = stopFor(santa.toCity);
    if (from && to) {
      // The API reports the two cities but not a live GPS coordinate; show the route midpoint.
      const longitudeDelta = ((((to.longitude - from.longitude) + 540) % 360) - 180);
      return [(from.latitude + to.latitude) / 2, from.longitude + longitudeDelta / 2];
    }
    if (from) return [from.latitude, from.longitude];
    return [90, 0];
  };

  const formatInputTime = (value) => {
    const date = new Date(value);
    if (Number.isNaN(date.valueOf())) return '';
    const local = new Date(date.valueOf() - date.getTimezoneOffset() * 60000);
    return local.toISOString().slice(0, 16);
  };

  async function loadLocation() {
    updatedText.textContent = pinnedTime ? 'Loading the selected UTC time…' : 'Refreshing live location…';
    const url = new URL(apiBase);
    if (pinnedTime) {
      url.pathname += '/location-test';
      url.searchParams.set('testDateTime', new Date(pinnedTime).toISOString());
    }
    try {
      const response = await fetch(url, { headers: { Accept: 'application/json' }, cache: 'no-store' });
      if (!response.ok) throw new Error(`API returned ${response.status}`);
      const santa = await response.json();
      const [latitude, longitude] = positionFor(santa);
      const label = santa.fromCity && santa.toCity
        ? `${santa.fromCity} to ${santa.toCity}`
        : santa.fromCity || (santa.status === 'Delivered' ? route.at(-1)?.city : 'North Pole') || 'North Pole';
      marker.setLatLng([latitude, longitude]).bindPopup(`<strong>Santa’s sleigh</strong><br>${label}`);
      map.setView([latitude, longitude], santa.status === 'NorthPole' ? 3 : 5);
      statusText.textContent = pinnedTime ? 'Test time' : 'Live tracking';
      locationText.textContent = label;
      descriptionText.textContent = santa.description || 'Santa is on his way.';
      statusDot.classList.toggle('is-live', !pinnedTime);
      updatedText.textContent = pinnedTime
        ? `Showing ${new Date(pinnedTime).toLocaleString(undefined, { timeZone: 'UTC', dateStyle: 'medium', timeStyle: 'short' })} UTC`
        : `Updated ${new Date().toLocaleTimeString(undefined, { hour: '2-digit', minute: '2-digit' })}`;
    } catch (error) {
      console.error('Unable to load Santa’s location:', error);
      setMessage('Could not connect to the Santa API.');
    }
  }

  fetch('/assets/santa-route.json')
    .then(response => {
      if (!response.ok) throw new Error(`Route data returned ${response.status}`);
      return response.json();
    })
    .then(data => {
      route = data.stops || [];
      if (requestedTime) {
        const date = new Date(requestedTime);
        if (Number.isNaN(date.valueOf())) {
          setMessage('The time in the URL is invalid. Use an ISO date and time, such as 2026-12-24T07:15:00Z.');
          timeHint.textContent = 'Use an ISO 8601 UTC time, for example 2026-12-24T07:15:00Z.';
          return;
        }
        timeInput.value = formatInputTime(requestedTime);
        clearTime.hidden = false;
      }
      loadLocation();
      if (!pinnedTime) window.setInterval(loadLocation, 60000);
    })
    .catch(error => {
      console.error('Unable to load route coordinates:', error);
      setMessage('Santa’s route data could not be loaded.');
    });

  document.querySelector('#time-form')?.addEventListener('submit', event => {
    event.preventDefault();
    if (!timeInput.value) {
      window.location.assign('/');
      return;
    }
    const date = new Date(timeInput.value);
    const shareUrl = new URL(window.location.href);
    shareUrl.searchParams.set('time', date.toISOString());
    window.location.assign(shareUrl.href);
  });
})();
