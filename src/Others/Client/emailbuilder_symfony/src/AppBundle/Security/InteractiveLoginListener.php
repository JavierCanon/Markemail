<?php 
namespace AppBundle\Security;

use Symfony\Component\HttpFoundation\RequestStack;
use Symfony\Component\Security\Http\Event\InteractiveLoginEvent;

use Doctrine\ORM\EntityManager;
use AppBundle\Entity\User;

class InteractiveLoginListener {

    protected $em;
    protected $request;

    public function __construct(EntityManager $em, RequestStack $request) {

        $this->em = $em;
        $this->request = $request;
    }

    public function onSecurityInteractiveLogin(InteractiveLoginEvent $event) {

        $user = $event->getAuthenticationToken()->getUser();

        if ($user instanceof User) {
            if($this->request->getCurrentRequest()->hasSession()) {
                $user->setLast_Login(new \DateTime('now'));
                $this->em->persist($user);
                $this->em->flush();
            }
        }
    }
}
