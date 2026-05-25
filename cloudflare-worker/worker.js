// KPP API Proxy — Cloudflare Worker
// Routes API calls through Cloudflare:
//   /api/gemini  → Gemini API (bypass "User location not supported")
//   /api/openrouter → OpenRouter API (hide API key from client)
// Deploy: https://dash.cloudflare.com → Workers & Pages → Create

const GEMINI_API_KEY = typeof process !== 'undefined'
  ? (process.env.GEMINI_API_KEY || '')
  : '';

const OPENROUTER_API_KEY = typeof process !== 'undefined'
  ? (process.env.OPENROUTER_KEY || '')
  : '';

export default {
  async fetch(request, env) {
    // Handle CORS preflight
    if (request.method === 'OPTIONS') {
      return new Response(null, {
        status: 204,
        headers: {
          'Access-Control-Allow-Origin': '*',
          'Access-Control-Allow-Methods': 'POST, OPTIONS',
          'Access-Control-Allow-Headers': 'Content-Type, X-Goog-Api-Key',
          'Access-Control-Max-Age': '86400'
        }
      });
    }

    // Only allow POST
    if (request.method !== 'POST') {
      return new Response('Method not allowed', { status: 405 });
    }

    const url = new URL(request.url);

    try {
      // Route: /api/gemini?model=...
      if (url.pathname.startsWith('/api/gemini')) {
        const model = url.searchParams.get('model') || 'gemini-2.0-flash-lite';
        const apiKey = env.GEMINI_API_KEY || GEMINI_API_KEY || request.headers.get('X-Goog-Api-Key');

        if (!apiKey) {
          return jsonResponse({ error: 'Missing Gemini API key' }, 400);
        }

        const geminiUrl = `https://generativelanguage.googleapis.com/v1beta/models/${model}:generateContent?key=${apiKey}`;
        const requestBody = await request.text();

        const geminiResponse = await fetch(geminiUrl, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: requestBody
        });

        const responseBody = await geminiResponse.text();

        return new Response(responseBody, {
          status: geminiResponse.status,
          headers: {
            'Content-Type': 'application/json',
            'Access-Control-Allow-Origin': '*'
          }
        });
      }

      // Route: /api/openrouter?model=...
      if (url.pathname.startsWith('/api/openrouter')) {
        const apiKey = env.OPENROUTER_KEY || OPENROUTER_API_KEY;

        if (!apiKey) {
          return jsonResponse({ error: 'Missing OpenRouter API key' }, 400);
        }

        const model = url.searchParams.get('model') || 'google/gemma-3-27b-it:free';
        const openrouterUrl = `https://openrouter.ai/api/v1/chat/completions`;
        const requestBody = await request.text();

        // Inject model if not present in body
        let body = requestBody;
        try {
          const parsed = JSON.parse(body);
          if (!parsed.model) {
            parsed.model = model;
            body = JSON.stringify(parsed);
          }
        } catch { /* use raw body */ }

        const openrouterResponse = await fetch(openrouterUrl, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${apiKey}`,
            'HTTP-Referer': 'https://aidarqa.github.io/kpp/',
            'X-Title': 'KPP Guest System'
          },
          body: body
        });

        const responseBody = await openrouterResponse.text();

        return new Response(responseBody, {
          status: openrouterResponse.status,
          headers: {
            'Content-Type': 'application/json',
            'Access-Control-Allow-Origin': '*'
          }
        });
      }

      // Legacy route (backward compatibility): direct proxy
      // This is the old behavior — forward as Gemini proxy
      const model = url.searchParams.get('model') || 'gemini-2.0-flash-lite';
      const apiKey = env.GEMINI_API_KEY || GEMINI_API_KEY || request.headers.get('X-Goog-Api-Key');

      if (!apiKey) {
        return jsonResponse({ error: 'Missing API key' }, 400);
      }

      const geminiUrl = `https://generativelanguage.googleapis.com/v1beta/models/${model}:generateContent?key=${apiKey}`;
      const requestBody = await request.text();

      const geminiResponse = await fetch(geminiUrl, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: requestBody
      });

      const responseBody = await geminiResponse.text();

      return new Response(responseBody, {
        status: geminiResponse.status,
        headers: {
          'Content-Type': 'application/json',
          'Access-Control-Allow-Origin': '*'
        }
      });

    } catch (err) {
      return jsonResponse({ error: err.message }, 500);
    }
  }
};

function jsonResponse(data, status) {
  return new Response(JSON.stringify(data), {
    status,
    headers: {
      'Content-Type': 'application/json',
      'Access-Control-Allow-Origin': '*'
    }
  });
}
