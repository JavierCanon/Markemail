<?php

namespace AppBundle\Controller;

use AppBundle\Entity\Emailproject;
use Symfony\Bundle\FrameworkBundle\Controller\Controller;
use Symfony\Component\HttpFoundation\Request;
use AppBundle\Entity\User;
use Symfony\Component\HttpFoundation\Response;

use AppBundle\Uploader;

//use Symfony\Component\Finder\Finder;


/**
 * Emailproject controller.
 *
 */
class EmailprojectController extends Controller
{
    /**
     * Lists all emailproject entities.
     *
     */
    public function indexAction()
    {
        $em = $this->getDoctrine()->getManager();
        $loggedInUser = $this->getUser();
        $username = $loggedInUser->getUsername();

        //$uploader = $this->get('app.uploader');
        //return $uploader->add(2,3);
        //$emailprojects = $em->getRepository('AppBundle:Emailproject')->findAll();
        $emailprojects = $em->getRepository('AppBundle:Emailproject')->loadUserProjects($username);

        return $this->render('emailproject/index.html.twig', array(
            'emailprojects' => $emailprojects,
        ));
    }

    /**
     * Creates a new emailproject entity.
     *
     */
    public function newAction(Request $request)
    {
        $d = new \DateTime();
        $emailproject = new Emailproject($request->get('body'),$request->get('title'), $d);
        $loggedInUser = $this->getUser();


        $emailproject
          ->setUsername($loggedInUser->getUsername())
          ->setEmail($loggedInUser->getEmail())
          ->setCompany($loggedInUser->getCompany());

        $form = $this->createForm('AppBundle\Form\EmailprojectType', $emailproject);
        $form->handleRequest($request);



       if ($form->isSubmitted() && $form->isValid()) {
            $em = $this->getDoctrine()->getManager();
            $em->persist($emailproject);
            $em->flush($emailproject);

            return $this->redirectToRoute('emailproject_show', array('id' => $emailproject->getId()));
        }

        return $this->render('emailproject/new.html.twig', array(
            'emailproject' => $emailproject,
            'form' => $form->createView(),
        ));
    }

    /**
     * Finds and displays a emailproject entity.
     *
     */
    public function showAction(Emailproject $emailproject)
    {
        $deleteForm = $this->createDeleteForm($emailproject);

        return $this->render('emailproject/show.html.twig', array(
            'emailproject' => $emailproject,
            'delete_form' => $deleteForm->createView(),
        ));
    }

    /**
     * Displays a form to edit an existing emailproject entity.
     *
     */
    public function editAction(Request $request, Emailproject $emailproject)
    {
        $deleteForm = $this->createDeleteForm($emailproject);
        $editForm = $this->createForm('AppBundle\Form\EmailprojectType', $emailproject);
        $editForm->handleRequest($request);

        if ($editForm->isSubmitted() && $editForm->isValid()) {
            $this->getDoctrine()->getManager()->flush();

            return $this->redirectToRoute('emailproject_index', array('id' => $emailproject->getId()));
        }

        return $this->render('emailproject/edit.html.twig', array(
            'emailproject' => $emailproject,
            'edit_form' => $editForm->createView(),
            'delete_form' => $deleteForm->createView(),
        ));
    }
    public function buildAction(Request $request, Emailproject $emailproject)
    {
        $company = $emailproject->getCompany();

        $json_file = file_get_contents($this->get('kernel')->getRootDir().'/Resources/views/company/sdta/_templates.json');
        $modules = json_decode($json_file,true);
        $moduleNames = array_keys($modules['templates']);


        return $this->render('emailproject/build.html', array(
            'emailproject' => $emailproject,
            'moduleNames' => $moduleNames,
        ));
    }
    public function ajaxAction(Request $request)
    {
      $loggedInUser = $this->getUser();
      $company = $loggedInUser->getCompany();

      $options = $request->request->get('options');

      switch($options['flag']){
        case "sdta-custom-insertSignature":
          $name=$options['name'];

          if($options['oddEven']=="true"){
            return $this->render('company/'.$company.'/templates/signatures.'.$name.'.html');

          }else{
            return $this->render('company/'.$company.'/templates/signatures.row.html',
              array('name' => $name)
            );

          }
          break;

        case "dropModule":
          $module = $options['module'];
          return $this->render('company/'.$company.'/templates/'.$module.'.lyt.php',
            array('module' => $module)
          );
          break;
        case "saveProject":
          $html = $options['html'];
          $projectID = $options['projectNum'];
          $em = $this->getDoctrine()->getEntityManager();

          $repo = $this->getDoctrine()->getRepository('AppBundle:Emailproject');
          $emailproject = $em->getRepository('AppBundle:Emailproject')->find($projectID);
          $emailproject->setBody($html);
          $em->flush($emailproject);
          return new Response();
          break;
        case "events-build":
          $module = $options['module'];
          $route = $options['route'];
          return $this->render('company/'.$company.'/templates/'.$module.'.php',array('route'=>'/emailproject/make_event_layout'));
          break;
        case "syncToExacttarget":
        $html = $options['html'];
        $etID = $options['etID'];
        break;
        default:break;
      }




    }
    public function get_htmlAction(Request $request){
      $options = $request->request->get('options');
      return $this->render($options['path']);
    }
    public function make_event_layoutAction(Request $request)
    {
      $loggedInUser = $this->getUser();
      $company = $loggedInUser->getCompany();
      $options = $request->request->get('options');
      //$params = array();
      parse_str($options,$params);

      $titles = $params["title"];
      $months = $params["month"];
      $dates = $params["date"];
      $urls = $params["dest_url"];
      $ad_contents = $params["ad_content"];

      $rows = [];

      foreach($titles as $index => $title){
        //next just need to see if title is empty, if so move on
        if($title == '') break;
        $x = new \stdClass();

        $x->title = $title;
        $x->month = $months[$index];
        $x->eventdate = $dates[$index];
        $x->url = $urls[$index];
        $x->adcontent = $ad_contents[$index];

        $rows[]=$x;

      }


      return $this->render('company/'.$company.'/templates/events.lyt.php',array(
          'module' => 'events',
          'rows'=> $rows )
            );
    }
    public function list_buildAction(Request $request)
    {
      //if there is a module option, then load the form
      $loggedInUser = $this->getUser();
      $company = $loggedInUser->getCompany();
      $options = $request->request->get('options');

      if(!empty($options['module'])){
        $module = $options['module'];
        $flag = $options['flag'];
        $route = $options['route'];
        $listItems = (!empty($options['listItems'])) ? $options['listItems'] : "empty" ;
        return $this->render('company/'.$company.'/templates/'.$module.'.php',array(
          'module' => $module,
          'action'=> $flag,
          'route'=> $route,
          'listItems'=> $listItems)
          );
      }else{

      parse_str($options,$params);

      $titles = $params["title"];
      $urls = $params["dest_url"];

      $rows = [];

      foreach($titles as $index => $title){
        //next just need to see if title is empty, if so move on
        if($title == '') break;
        $x = new \stdClass();

        $x->title = $title;
        $x->url = $urls[$index];

        $rows[]=$x;

      }


      return $this->render('company/'.$company.'/templates/paragraph.lyt.php',array(
          'module' => 'simple-list',
          'rows'=> $rows ));
      }
    }
    /**
     * Deletes a emailproject entity.
     *
     */
    public function deleteAction(Request $request, Emailproject $emailproject)
    {
        $form = $this->createDeleteForm($emailproject);
        $form->handleRequest($request);

        if ($form->isSubmitted() && $form->isValid()) {
            $em = $this->getDoctrine()->getManager();
            $em->remove($emailproject);
            $em->flush($emailproject);
        }

        return $this->redirectToRoute('emailproject_index');
    }

    public function deleteMultipleAction(Request $request)
    {
      $inputs = $request->get('delete');
      $em = $this->getDoctrine()->getManager();

      foreach($inputs as $projectID){
        $emailproject = $em->getRepository('AppBundle:Emailproject')->find($projectID);
        $em->remove($emailproject);
        $em->flush($emailproject);

      }

        return $this->redirectToRoute('emailproject_index');

  }



    /**
     * Creates a form to delete a emailproject entity.
     *
     * @param Emailproject $emailproject The emailproject entity
     *
     * @return \Symfony\Component\Form\Form The form
     */
    private function createDeleteForm(Emailproject $emailproject)
    {
        return $this->createFormBuilder()
            ->setAction($this->generateUrl('emailproject_delete', array('id' => $emailproject->getId())))
            ->setMethod('DELETE')
            ->getForm()
        ;
    }
}
