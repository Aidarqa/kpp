// KPP Gemini Proxy — Cloudflare Worker
// Routes Gemini API calls through Cloudflare to bypass "User location not supported"
// Deploy: https://dash.cloudflare.com → Workers & Pages → Create

export default {
  async fetch(request) {
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

    try {
      const url = new URL(request.url);
      const model = url.searchParams.get('model') || 'gemini-2.0-flash-lite';

      // Get API key from client header
      const apiKey = request.headers.get('X-Goog-Api-Key');
      if (!apiKey) {
        return jsonResponse({ error: 'Missing X-Goog-Api-Key header' }, 400);
      }

      // Forward request to Gemini API
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
