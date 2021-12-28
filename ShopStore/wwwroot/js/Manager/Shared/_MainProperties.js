
const MainProperties = {
    //訂單管理
    Order: {
        data: [
            {
                num: '',
                memberId: '',
                memberName: '',
                date: '',
                status: '',
                statusBadge: '',
                shippingMethod: '',
                shippingBadge: '',
                total: 0,
                isDel: 0,
                isPaid: 0
            }],

        postData: [
            {
                f_id: '',
                f_status: 0,
                f_shippingMethod: 0
            }],

        /*切換狀態*/
        STATUS: {
            tbd: 1,
            shipped: 2,
            transport: 3,
            returned: 4,
            chanceled: 5,
            abnormal: 6
        },

        SipMethod: {
            postm: 1,
            B2B: 2,
            privateCargo: 3
        }
        /*切換狀態*/
    },

    //帳號管理
    User: {
        data: [{
            id: 0,
            name: '',
            account: '',
            groupId: 0,
            groupName: 'Admin',
            createTime: '',
            updateTime: '',
            //userPermissions: []

            //{
            //    f_groupId: 0,
            //    menuName: '',
            //    permissionCode: 8
            //}
        }],

        group: {
            '1': 'Admin',
            '2': 'Normal'
        },

        //傳入GroupId 回傳可操作的menuSub
        group_default_permission: {
            '1': {
                '產品管理': 0,
                '會員管理': 0,
                '訂單管理': 0,
                '測試用': 0
            },

            '2': {
                '產品管理': 0,
            }
        },

        user_permission: {

        }
    }
}