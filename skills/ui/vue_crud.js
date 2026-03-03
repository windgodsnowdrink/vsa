#:sdk none
#:package axios@1.6.2
#:package vue@3.4.0

import { ref, onMounted } from 'vue'
import axios from 'axios'

export default {
  setup() {
    const items = ref([])
    const currentItem = ref({})
    const properties = ref([])
    
    const loadData = async () => {
      const res = await axios.get(`/api/${entityName}`)
      items.value = res.data
      properties.value = Object.keys(items.value[0] || {})
    }
    
    const saveItem = async () => {
      if(currentItem.value.id) {
        await axios.put(`/api/${entityName}/${currentItem.value.id}`, currentItem.value)
      } else {
        await axios.post(`/api/${entityName}`, currentItem.value)
      }
      await loadData()
      currentItem.value = {}
    }
    
    const deleteItem = async (id) => {
      await axios.delete(`/api/${entityName}/${id}`)
      await loadData()
    }
    
    onMounted(loadData)
    
    return { items, currentItem, properties, saveItem, deleteItem }
  }
}