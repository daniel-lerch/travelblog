import client from './client'

export interface SubscribeRequest {
    mailAddress: string;
    givenName: string;
    familyName: string;
    comment: string;
}

export async function subscribe (request: SubscribeRequest): Promise<boolean> {
  const response = await client.post('/api/subscribe', request)
  return response.status === 204
}
