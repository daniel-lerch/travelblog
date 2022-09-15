<template>
  <h1>Registrierung</h1>

  <form @submit.prevent="register">
    <div class="form-group">
      <label for="givenname">Vorname</label>
      <input type="text" class="form-control" id="givenname" v-model="givenName" required autocomplete="given-name" />
    </div>
    <div class="form-group">
      <label for="familyname">Nachname</label>
      <input type="text" class="form-control" id="familyname" v-model="familyName" required
        autocomplete="family-name" />
    </div>
    <div class="form-group">
      <label for="mailaddress">E-Mail-Adresse</label>
      <input type="email" class="form-control" id="mailaddress" v-model="mailAddress" required autocomplete="email" />
      <small class="form-text text-muted">Deine E-Mail-Adresse wird ausschließlich für diesen Newsletter verwendet
        und nicht an Dritte weitergeben.</small>
    </div>
    <button type="submit" class="btn btn-primary">Registrieren</button>
  </form>
</template>

<script lang="ts">
import router from '@/router'
import { defineComponent, ref } from 'vue'

export default defineComponent({
  setup () {
    const givenName = ref('')
    const familyName = ref('')
    const mailAddress = ref('')

    function register () {
      // TODO: Perform web request and redirect
      router.push({ name: 'Registered' })
    }

    return { givenName, familyName, mailAddress, register }
  }
})
</script>
