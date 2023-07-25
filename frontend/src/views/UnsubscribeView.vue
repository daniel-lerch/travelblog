<template>
  <h1 v-if="state === 'INVALID'">Ungültiger Link</h1>
  <h1 v-else-if="state === 'SUCCESS'">E-Mails abbestellt</h1>
  <h1 v-else>E-Mails abbestellen</h1>

  <div v-if="state === 'NETWORK_FAILURE'" class="alert alert-danger alert-dismissible fade show" role="alert">
    <strong>Netzwerkfehler.</strong> Bitte überprüfe deine Internetverbindung.
    <button type="button" class="close" data-dismiss="alert" aria-label="Close">
      <span aria-hidden="true">&times;</span>
    </button>
  </div>

  <p v-if="state === 'INVALID'">
    Dieser Link ist leider ungültig. Bitte klicke auf den Link in der letzten E-Mail.
    Falls diese Meldung dann immer noch erscheint, hast dich bereits abgemeldet.
  </p>
  <p v-else-if="state === 'SUCCESS'">
    Du wirst keine weiteren E-Mails von diesem Blog an {{ mailAddress }} mehr erhalten.
  </p>
  <p v-else>
    Hey {{ givenName }},<br />
    willst du wirklich keine E-Mails zu neuen Einträgen auf diesem Blog mehr bekommen und damit auch den Zugang dazu
    verlieren?
  </p>

  <form v-if="state !== 'INVALID' && state !== 'SUCCESS'" @submit.prevent="sendUnsubscribe">
    <button type="submit" class="btn btn-primary" :disabled="state === 'PROCESSING'">Abbestellen</button>
  </form>
</template>

<script lang="ts">
import { defineComponent, onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import { profile, unsubscribe } from '@/api/subscriber'

export default defineComponent({
  setup () {
    const state = ref<'LOADING' | 'IDLE' | 'PROCESSING' | 'SUCCESS' | 'INVALID' | 'NETWORK_FAILURE'>('LOADING')
    const mailAddress = ref('')
    const givenName = ref('')
    let token: string

    onMounted(async () => {
      const route = useRoute()
      token = route.query.token as string
      const response = await profile(token)
      if (response === undefined) {
        state.value = 'NETWORK_FAILURE'
      } else if (response === null) {
        state.value = 'INVALID'
      } else {
        mailAddress.value = response.mailAddress
        givenName.value = response.givenName
        state.value = 'IDLE'
      }
    })

    async function sendUnsubscribe () {
      state.value = 'PROCESSING'
      const response = await unsubscribe(token)
      if (response === true) {
        state.value = 'SUCCESS'
      } else if (response === false) {
        state.value = 'INVALID'
      } else {
        state.value = 'NETWORK_FAILURE'
      }
    }

    return { state, mailAddress, givenName, sendUnsubscribe }
  }
})
</script>
