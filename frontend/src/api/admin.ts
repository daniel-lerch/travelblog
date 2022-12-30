import { get } from './client'

export interface Subscriber {
    id: number;
    mailAddress: string | null;
    givenName: string
    familyName: string
    confirmationTime: Date | null;
    deletionTime: Date | null;
}

export function getSubscribers (): Promise<Subscriber[]> {
  return get('/api/admin/subscribers')
}
