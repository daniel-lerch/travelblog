import axios from 'axios'

const baseUrl =
    process.env.VUE_APP_API_URL ?? window.resourceBasePath.slice(0, -1)

function jsonParse (text: string) {
  return JSON.parse(text, (key, value) => {
    if (key.endsWith('Time') && value !== null) {
      return new Date(value)
    } else {
      return value
    }
  })
}

export async function get<T> (query: string): Promise<T> {
  const response = await fetch(baseUrl + query)
  if (response.ok === false) {
    throw new Error('Unexpected status code ' + response.status)
  }
  const responseText = await response.text()
  return jsonParse(responseText) as T
}

export default axios.create({
  baseURL: process.env.VUE_APP_API_URL ?? window.resourceBasePath.slice(0, -1),
  // Disable Axios throwing errors for non-success status codes because handling is easier this way
  validateStatus: status => status >= 200 && status < 600
})
