(() => {
  const refreshIntervalMs = 60_000;
  const parameters = new URLSearchParams(window.location.search);
  const requestedLanguage = parameters.get('lang') || 'en-US';
  const language = ['en-US', 'es-ES'].includes(requestedLanguage) ? requestedLanguage : 'en-US';
  const requestedTheme = parameters.get('theme');
  const theme = requestedTheme === 'light' ? 'light' : 'night';
  const apiUrl = new URL(
    window.location.hostname === 'localhost'
      ? 'http://localhost:5146/api/santa'
      : 'https://api.sleighwatch.com/api/santa');
  const copy = {
    'en-US': {
      setsOffToday: 'Santa sets off today',
      setsOffTomorrow: 'Santa sets off tomorrow',
      setsOffInDays: days => `Santa sets off in ${days} days`,
      liveStatus: 'Live Santa status',
      journeyComplete: 'Santa is on his way home'
    },
    'es-ES': {
      setsOffToday: 'Santa parte hoy',
      setsOffTomorrow: 'Santa parte mañana',
      setsOffInDays: days => `Santa parte en ${days} días`,
      liveStatus: 'Estado de Santa en directo',
      journeyComplete: 'Santa está de camino a casa'
    }
  }[language];

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

  const getStatusMessage = santa => {
    const departureAt = Date.parse(santa.departureAtUtc);
    const daysUntilDeparture = santa.daysUntilDeparture;

    if (Number.isInteger(daysUntilDeparture) && departureAt > Date.now()) {
      if (daysUntilDeparture === 0) {
        return copy.setsOffToday;
      }

      if (daysUntilDeparture === 1) {
        return copy.setsOffTomorrow;
      }

      return copy.setsOffInDays(daysUntilDeparture);
    }

    if (santa.status === 'InFlight') {
      return copy.liveStatus;
    }

    return santa.status === 'Delivered' ? copy.journeyComplete : santa.status;
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
      status.textContent = getStatusMessage(santa);
    } catch (error) {
      setErrorState();
      console.error('Unable to load SleighWatch widget data.', error);
    }
  };

  loadSanta();
  window.setInterval(loadSanta, refreshIntervalMs);
})();
