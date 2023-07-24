<template>
  <h1 v-if="state !== 'SUCCESS'">Registrierung</h1>
  <h1 v-else>Registrierung abgeschlossen</h1>

  <div v-if="state === 'NETWORK_FAILURE'" class="alert alert-danger alert-dismissible fade show" role="alert">
    <strong>Netzwerkfehler.</strong> Bitte überprüfe deine Internetverbindung.
    <button type="button" class="close" data-dismiss="alert" aria-label="Close">
      <span aria-hidden="true">&times;</span>
    </button>
  </div>

  <div v-if="state === 'CONFLICT'" class="alert alert-danger alert-dismissible fade show" role="alert">
    <strong>Registrierung fehlgeschlagen.</strong> Du bist mit dieser E-Mail-Adresse bereits registriert.
    <button type="button" class="close" data-dismiss="alert" aria-label="Close">
      <span aria-hidden="true">&times;</span>
    </button>
  </div>

  <form v-if="state !== 'SUCCESS'" @submit.prevent="register">
    <div class="form-group">
      <label for="givenname">Vorname</label>
      <input type="text" class="form-control" id="givenname" v-model="givenName" required autocomplete="given-name" />
    </div>
    <div class="form-group">
      <label for="familyname">Nachname</label>
      <input type="text" class="form-control" id="familyname" v-model="familyName" required autocomplete="family-name" />
    </div>
    <div class="form-group">
      <label for="mailaddress">E-Mail-Adresse</label>
      <input type="email" class="form-control" id="mailaddress" v-model="mailAddress" required autocomplete="email" />
      <small class="form-text text-muted">Deine E-Mail-Adresse wird ausschließlich für diesen Newsletter verwendet
        und nicht an Dritte weitergeben.</small>
    </div>
    <button type="submit" class="btn btn-primary" :disabled="state === 'PROCESSING'">Registrieren</button>
  </form>

  <p v-else>
    Hey {{ givenName }},<br />
    Du hast dich erfolgreich für den Newsletter registriert.
    Sobald deine Registrierung bestätigt wird, wirst du per E-Mail darüber informiert.
    Dann bekommst du für jeden neuen Blogeintrag eine Erinnerung an {{ mailAddress }} gesendet.
  </p>
</template>

<script lang="ts">
import { defineComponent, ref } from 'vue'
import { subscribe } from '@/api/subscriber'

export default defineComponent({
  setup () {
    const givenName = ref('')
    const familyName = ref('')
    const mailAddress = ref('')
    const state = ref<'IDLE' | 'PROCESSING' | 'SUCCESS' | 'CONFLICT' | 'NETWORK_FAILURE'>('IDLE')

    async function register () {
      state.value = 'PROCESSING'
      const response = await subscribe({ givenName: givenName.value, familyName: familyName.value, mailAddress: mailAddress.value, comment: '' })
      if (response === true) {
        state.value = 'SUCCESS'
      } else if (response === false) {
        state.value = 'CONFLICT'
      } else {
        state.value = 'NETWORK_FAILURE'
      }
    }

    return { givenName, familyName, mailAddress, state, register }
  }
})
</script>
