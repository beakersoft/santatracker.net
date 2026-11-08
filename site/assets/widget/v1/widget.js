(() => {
  const refreshIntervalMs = 60_000;
  const parameters = new URLSearchParams(window.location.search);
  const requestedLanguage = parameters.get('lang') || 'en-US';
  const language = ['en-US', 'es-ES'].includes(requestedLanguage) ? requestedLanguage : 'en-US';
  const requestedTheme = parameters.get('theme');
  const theme = requestedTheme === 'light' ? 'light' : 'night';
  const apiUrl = new URL('https://api.sleighwatch.com/api/santa');

  apiUrl.searchParams.set('lang', language);
  document.documentElement.dataset.theme = theme;

  const widget = document.querySelector('.widget');
  const description = document.querySelector('#widget-description');
  const status = document.querySelector('#widget-status');

  const setErrorState = () => {
    widget.dataset.state = 'error';
    description.textContent = 'Santa’s location is temporarily unavailable.';
    status.textContent = 'Please try again in a moment.';
  };

  const loadSanta = async () => {
    try {
      const response = await fetch(apiUrl, { headers: { Accept: 'application/json' } });
      if (!response.ok) {
        throw new Error(`Santa API returned ${response.status}.`);
      }

      const santa = await response.json();
      widget.dataset.state = 'ready';
      description.textContent = santa.description;
      status.textContent = santa.status === 'InFlight' ? 'Live Santa status' : santa.status;
    } catch (error) {
      setErrorState();
      console.error('Unable to load SleighWatch widget data.', error);
    }
  };

  loadSanta();
  window.setInterval(loadSanta, refreshIntervalMs);
})();
